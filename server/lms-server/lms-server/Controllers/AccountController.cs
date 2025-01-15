using lms.Abstractions.Models;
using lms.Abstractions.Models.DTO;
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

        public AccountController(UserManager<Client> userManager, SignInManager<Client> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;

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
                        return Ok(loginResult);
                    }
                    return BadRequest();
                }
                return NotFound();
            }
            catch(Exception ex)
            {
                Console.Error.WriteLine($"Error in AccountController: {ex}");
                throw;
            }

        }

        //[HttpPut("ChangePassword")]
        //public async Task<IActionResult> changePassword()
    }
}
