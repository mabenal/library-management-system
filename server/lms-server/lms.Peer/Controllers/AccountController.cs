using lms.Abstractions.Interfaces;
using lms.Abstractions.Models;
using lms.Abstractions.Models.DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace lms.Peer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private SignInManager<ApplicationUser> signInManager;

        public ITokenRepository tokenRepository { get; }

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
                    return Ok(registerResponse);
                }
                else
                {
                    return BadRequest(registrationResult.Errors);
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error in AccountController: {ex}");
                throw;
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

                        var token = tokenRepository.CreateJWTToken(user);

                        var loginResponseObject = new LoginResponseDto
                        {

                             Token =  token,
                             Username = loginObject.UserName
                        };
                        return Ok(loginResponseObject);
                    }


                    return BadRequest(new { Message = "Invalid credentials" });
                }
                    return NotFound(new { Message = "User not found" });
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error in AccountController: {ex}");
                return StatusCode(500, new { Message = "An error occurred during login" });
            }

        }


        [HttpPut("AssignRole")]
        public async Task<ActionResult<AccountActionResponseDto>> AssignRole([FromBody] RoleDto assignRoleDto)
        {
            var user = await userManager.FindByIdAsync(assignRoleDto.UserId.ToString());
            if (user == null)
            {
                return NotFound(new { Message = "User not found" });
            }


            
            var result = await userManager.AddToRoleAsync(user, assignRoleDto.Role);

            var assignRoleResponse = new AccountActionResponseDto
            {
                isSuccessful = result.Succeeded,
                errors=result.Errors
            };


            if (result.Succeeded)
            {
                return Ok(assignRoleResponse);
            }

            return BadRequest(assignRoleResponse);
        }

        [HttpPut("RemoveRole")]
        public async Task<IActionResult> RemoveRole([FromBody] RoleDto roleModel)
        {
            var user = await userManager.FindByIdAsync(roleModel.UserId.ToString());
            if (user == null)
            {
                return NotFound(new { Message = "User not found" });
            }
            var result = await userManager.RemoveFromRoleAsync(user, roleModel.Role);


            var removeRoleResponse = new AccountActionResponseDto
            {
                isSuccessful = result.Succeeded,
                errors = result.Errors
            };

            if (result.Succeeded)
            {
                return Ok(removeRoleResponse);
            }

            return BadRequest(result.Errors);

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

                return BadRequest(result.Errors);


            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error in AccountController: {ex}");
                throw;
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

                return BadRequest(result.Errors);

            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error in AccountController: {ex}");
                throw;
            }
        }

    }
}