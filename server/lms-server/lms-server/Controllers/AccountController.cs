using lms.Abstractions.Models;
using lms.Abstractions.Models.DTO;
using lms_server.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace lms_server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<Client> userManager;
        private SignInManager<Client> signInManager;

        public ITokenRepository tokenRepository { get; }

        public AccountController(UserManager<Client> userManager, SignInManager<Client> signInManager, ITokenRepository tokenRepository)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.tokenRepository = tokenRepository;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> RegisterNewUser([FromBody] RegisterDto registerModel)
        {
            try
            {
                var client = new Client
                {
                    UserName = registerModel.Email,
                    Email = registerModel.Email,
                    Name = registerModel.Name,
                    LastName = registerModel.LastName,
                    Address = registerModel.Address,
                    PhoneNumber = registerModel.PhoneNumber
                };

                var registrationResult = await userManager.CreateAsync(client, registerModel.Password);

                if (registrationResult.Succeeded)
                {
                    return Ok(registrationResult);
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
        public async Task<IActionResult> Login([FromBody] LoginDto loginObject)
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
                        return Ok(new
                        {
                            Token = token,
                            Username = user.UserName,
                            ExpiresIn= 86400
                        });
                    }
                    return BadRequest(new { Message = "Invalid credentials" });
                }
                return NotFound(new {Message = "User not found"});
            }
            catch(Exception ex)
            {
                Console.Error.WriteLine($"Error in AccountController: {ex}");
                return StatusCode(500, new { Message = "An error occurred during login" });
            }

        }


        [HttpPost("AssignRole")]
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleDto assignRoleDto)
        {
            var user = await userManager.FindByIdAsync(assignRoleDto.UserId.ToString());
            if (user == null)
            {
                return NotFound(new { Message = "User not found" });
            }

            var result = await userManager.AddToRoleAsync(user, assignRoleDto.Role);
            if (result.Succeeded)
            {
                return Ok(new { Message = "Role assigned successfully" });
            }

            return BadRequest(new { Message = "Failed to assign role", Errors = result.Errors });
        }

        [HttpPut("ChangePassword")]

        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDto changePasswordRequestDto)
        {
            try
            {
                var user = await userManager.FindByNameAsync(changePasswordRequestDto.Username);

                if(user == null)
                {
                    return NotFound();
                }

                var result = await userManager.ChangePasswordAsync(user, changePasswordRequestDto.CurrentPassword, changePasswordRequestDto.NewPassword);

                if (result.Succeeded)
                {
                    return Ok();

                }

                return BadRequest(result.Errors);


            }catch( Exception ex)
            {
                Console.Error.WriteLine($"Error in AccountController: {ex}");
                throw;
            }
        }

    }
 }

