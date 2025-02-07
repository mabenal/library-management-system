using lms.Abstractions.Exceptions;
using lms.Abstractions.Interfaces;
using lms.Abstractions.Models;
using lms.Abstractions.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace lms.Peer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private SignInManager<ApplicationUser> signInManager;
        private readonly ILmsDbContext dbContext;

        public ITokenRepository tokenRepository { get; set; }

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ITokenRepository tokenRepository, ILmsDbContext dbContext)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.tokenRepository = tokenRepository;
            this.dbContext = dbContext;
        }

        [HttpPost("Register")]
        public async Task<ActionResult<RegisterResponseDto>> RegisterNewUser([FromBody] RegisterDto registerModel)
        {
            try
            {
                var user = new ApplicationUser
                {
                    UserName = registerModel.Email,
                    Email = registerModel.Email,
                    Name = registerModel.Name,
                    LastName = registerModel.LastName,
                    Address = registerModel.Address,
                    PhoneNumber = registerModel.PhoneNumber
                };

                var registrationResult = await userManager.CreateAsync(user, registerModel.Password);

                var registerResponse = new RegisterResponseDto
                {
                    Succeeded = registrationResult.Succeeded,
                    Errors = registrationResult.Errors
                };

                if (registrationResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "client");

                    var client = new Client
                    {
                        Id = user.Id,
                        Name = user.Name,
                        LastName = user.LastName,
                        EmailAddress = user.Email,
                        Password = registerModel.Password,
                        Address = user.Address,
                        PhoneNumber = user.PhoneNumber
                    };

                    await dbContext.Clients.AddAsync(client);
                    await dbContext.SaveChangesAsync();
                    return Ok(registerResponse);
                }
                else
                {
                    return BadRequest(registrationResult.Errors);
                }
            }
            catch (GlobalException ex)
            {
                throw new GlobalException($"in accountController: {ex}");
            }
            catch (Exception ex)
            {
                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "An internal server error occurred. Please try again later.",
                };
                return StatusCode(StatusCodes.Status500InternalServerError, problemDetails);
            }
        }

        [HttpPost("Login")]
        public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginDto loginObject)
        {
            try
            {
                var user = await userManager.FindByNameAsync(loginObject.UserName);

                if (user != null)
                {
                    var loginResult = await signInManager.PasswordSignInAsync(user, loginObject.Password, isPersistent: false, lockoutOnFailure: false);

                    if (loginResult.Succeeded)
                    {
                        var userRoles = await userManager.GetRolesAsync(user);
                        var token = await tokenRepository.CreateJWTTokenAsync(user);

                        var loginResponseObject = new LoginResponseDto
                        {

                             Token =  token,
                             Username = user.UserName,
                             UserRoles = userRoles.ToList(),
                             UserID = user.Id.ToString()
                        };

                        return Ok(loginResponseObject);
                    }

                    var userproblemDetails = new ProblemDetails
                    {
                        Status = StatusCodes.Status500InternalServerError,
                        Title = "An internal server error occurred. Please try again later."
                    };

                    return BadRequest(userproblemDetails);
                }

                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "An internal server error occurred. Please try again later."
                };

                return NotFound(problemDetails);
            }
            catch (GlobalException ex)
            {
                throw new GlobalException($"in accountController: {ex}");
            }
            catch (Exception ex)
            {
                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "An internal server error occurred. Please try again later."
                };
                return StatusCode(StatusCodes.Status500InternalServerError, problemDetails);
            }
        }

        [Authorize(Roles = "admin")]
        [HttpPost("AssignRole/{clientId}/{role}")]
        public async Task<ActionResult<AccountActionResponseDto>> AssignRole([FromRoute] Guid clientId, [FromRoute] string role)
        {
            try
            {
                var user = await userManager.FindByIdAsync(clientId.ToString());
                if (user == null)
                {
                    var problemDetails = new ProblemDetails
                    {
                        Status = StatusCodes.Status404NotFound,
                        Title = "User not found"
                    };
                    return NotFound(problemDetails);
                }

                var currentRoles = await userManager.GetRolesAsync(user);
                if (currentRoles.Any())
                {
                    await userManager.RemoveFromRolesAsync(user, currentRoles.ToArray());
                }

                var result = await userManager.AddToRoleAsync(user, role);
                var assignRoleResponse = new AccountActionResponseDto
                {
                    isSuccessful = result.Succeeded,
                    errors = result.Errors
                };

                if (result.Succeeded)
                {
                    if (role != "Client")
                    {
                        var client = await dbContext.Clients.FindAsync(user.Id);
                        if (client != null)
                        {
                            dbContext.Clients.Remove(client);
                            await dbContext.SaveChangesAsync();
                        }
                    }
                    return Ok(assignRoleResponse);
                }

                return BadRequest(assignRoleResponse);
            }
            catch (GlobalException ex)
            {
                throw new GlobalException($"in accountController: {ex}");
            }
            catch (Exception)
            {
                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "An internal server error occurred. Please try again later."
                };
                return StatusCode(StatusCodes.Status500InternalServerError, problemDetails);
            }
        }

        [Authorize(Roles = "admin")]
        [HttpPut("RemoveRole")]
        public async Task<ActionResult<AccountActionResponseDto>> RemoveRole([FromBody] RoleDto roleModel)
        {
            try
            {
                var user = await userManager.FindByIdAsync(roleModel.UserId.ToString());
                if (user == null)
                {
                    var problemDetails = new ProblemDetails
                    {
                        Status = StatusCodes.Status404NotFound,
                        Title = "User not found"
                    };
                    return NotFound(problemDetails);
                }

                var result = await userManager.RemoveFromRoleAsync(user, roleModel.Role);
                var response = new AccountActionResponseDto
                {
                    isSuccessful = result.Succeeded,
                    errors = result.Errors
                };

                if (result.Succeeded)
                {
                    return Ok(response);
                }

                return BadRequest(response);
            }
            catch (GlobalException ex)
            {
                throw new GlobalException($"in accountController: {ex}");
            }
            catch (Exception ex)
            {
                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "An internal server error occurred. Please try again later."
                };
                return StatusCode(StatusCodes.Status500InternalServerError, problemDetails);
            }
        }

        [HttpPut("ChangePassword")]

        public async Task<ActionResult<AccountActionResponseDto>> ChangePassword([FromBody] ChangePasswordRequestDto changePasswordRequestDto)
        {
            try
            {
                var user = await userManager.FindByNameAsync(changePasswordRequestDto.Username);

                if (user == null)
                {
                    return NotFound();
                }

                var result = await userManager.ChangePasswordAsync(user, changePasswordRequestDto.CurrentPassword, changePasswordRequestDto.NewPassword);


                var changePasswordResponse = new AccountActionResponseDto
                {
                    isSuccessful = result.Succeeded,
                    errors = result.Errors
                };

                if (result.Succeeded)
                {
                    return Ok(changePasswordResponse);

                }

                return BadRequest(result);
            }
            catch (GlobalException ex)
            {
                throw new GlobalException($"in accountController: {ex}");
            }
            catch (Exception ex)
            {
                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "An error occured when changing password",
                };
                return StatusCode(StatusCodes.Status500InternalServerError, problemDetails);
            }
        }


        [HttpDelete("DeleteUser/{id:Guid}")]

        public async Task<ActionResult<AccountActionResponseDto>> DeleteUser([FromRoute] Guid id)
        {
            try
            {
                var user = await userManager.FindByIdAsync(id.ToString());

                if (user == null)
                {
                    return NotFound();
                }

                var result = await userManager.DeleteAsync(user);

                var changePasswordResponse = new AccountActionResponseDto
                {
                    isSuccessful = result.Succeeded,
                    errors = result.Errors
                };

                if (result.Succeeded)
                {
                    var client = await dbContext.Clients.FindAsync(user.Id);
                    if (client != null)
                    {
                        dbContext.Clients.Remove(client);
                        await dbContext.SaveChangesAsync();
                    }
                    return Ok(changePasswordResponse);
                }

                return BadRequest(changePasswordResponse);

            }
            catch (GlobalException ex)
            {
                throw new GlobalException($"in accountController: {ex}");
            }
            catch (Exception ex)
            {
                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "An error occurred when deleting user",
                };
                return StatusCode(StatusCodes.Status500InternalServerError, problemDetails);
            }
        }

        [Authorize(Roles = "admin")]
        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var allUsers = await userManager.Users.ToListAsync();

                if (allUsers == null || !allUsers.Any())
                {
                    return NotFound();
                }

                return Ok(allUsers);
            }
            catch (GlobalException ex)
            {
                throw new GlobalException($"in accountController: {ex}");
            }
            catch (Exception ex)
            {
                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "An error occurred when getting all users",
                };
                return StatusCode(StatusCodes.Status500InternalServerError, problemDetails);
            }
        }

        [HttpPut("UpdateProfile/{id:Guid}")]
        public async Task<ActionResult<AccountActionResponseDto>> UpdateUserProfile([FromBody] UpdateUserRequestDto updateUserRequestDto, [FromRoute] Guid id)
        {
            try
            {
                if (updateUserRequestDto == null)
                {
                    var problemDetails = new ProblemDetails
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Title = "Invalid user data",
                    };
                    return BadRequest(problemDetails);
                }

                var user = await userManager.FindByIdAsync(id.ToString());
                if (user == null)
                {
                    var problemDetails = new ProblemDetails
                    {
                        Status = StatusCodes.Status404NotFound,
                        Title = "User not found",
                    };
                    return NotFound(problemDetails);
                }

                user.Name = updateUserRequestDto.Name;
                user.LastName = updateUserRequestDto.LastName;
                user.PhoneNumber = updateUserRequestDto.PhoneNumber;
                user.Address = updateUserRequestDto.Address;

                var result = await userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    var problemDetails = new ProblemDetails
                    {
                        Status = StatusCodes.Status304NotModified,
                        Title = "Failed to update user profile",
                        Detail = result?.Errors?.FirstOrDefault()?.Description
                    };
                    return BadRequest(problemDetails);
                }
                var updateProfileResponse = new AccountActionResponseDto
                {
                    isSuccessful = result.Succeeded,
                    errors = result.Errors
                };
                return Ok(updateProfileResponse);

            }
            catch (GlobalException ex)
            {
                throw new GlobalException($"in accountController: {ex}");
            }
            catch (Exception ex)
            {
                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "An error occurred when updating user profile"
                };
                return StatusCode(StatusCodes.Status500InternalServerError, problemDetails);
            }
        }

        [HttpGet("GetProfile/{id:Guid}")]
        public async Task<ActionResult<ApplicationUser>> GetUserProfile([FromRoute] Guid id)
        {
            try
            {
                var user = await userManager.FindByIdAsync(id.ToString());

                if(user != null)
                {
                    return Ok(user);
                }
                return NotFound();
            }
            catch (GlobalException ex)
            {
                throw new GlobalException($"in accountController: {ex}");
            }
            catch (Exception ex)
            {
                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "An error occurred when getting user profile"
                };
                return StatusCode(StatusCodes.Status500InternalServerError, problemDetails);
            }
        }   

    }


}