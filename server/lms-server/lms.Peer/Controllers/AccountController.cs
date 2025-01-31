using lms.Abstractions.Interfaces;
using lms.Abstractions.Models;
using lms.Abstractions.Models.DTO;
using lms.Abstractions.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace lms.Peer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private SignInManager<ApplicationUser> signInManager;

        public ITokenRepository tokenRepository { get; set; }

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ITokenRepository tokenRepository)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.tokenRepository = tokenRepository;
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
                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails { Title = "An internal server error occurred. Please try again later." });
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

                            Token = token,
                            Username = loginObject.UserName,
                            UserRoles = userRoles.ToList()
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
        [HttpPost("AssignRole")]
        public async Task<ActionResult<AccountActionResponseDto>> AssignRole([FromBody] RoleDto assignRoleDto)
        {
            try
            {
                var user = await userManager.FindByIdAsync(assignRoleDto.UserId.ToString());
                if (user == null)
                {
                    var problemDetails = new ProblemDetails
                    {
                        Status = StatusCodes.Status404NotFound,
                        Title = "User not found"
                    };
                    return NotFound(problemDetails);
                }

                var result = await userManager.AddToRoleAsync(user, assignRoleDto.Role);
                var assignRoleResponse = new AccountActionResponseDto
                {
                    isSuccessful = result.Succeeded,
                    errors = result.Errors
                };

                if (result.Succeeded)
                {
                    return Ok(assignRoleResponse);
                }

                return BadRequest(assignRoleResponse);
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
                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails { Title = "An error occured when changing password" });
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
                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails { Title = "An error occurred when deleting userd" });
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
                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails { Title = "An error occurred when getting all users" });
            }
        }


    }
}