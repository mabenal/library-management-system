﻿using lms.Abstractions.Interfaces;
using lms.Abstractions.Models;
using lms.Abstractions.Models.DTO;
using lms.Abstractions.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Net.WebSockets;

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
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error in AccountController: {ex}");
                return StatusCode(500, new { Message = "An internal server error occurred. Please try again later." });
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
                             UserID= user.Id.ToString()
                        };

                        return Ok(loginResponseObject);
                    }

                    return BadRequest(new { Message = "Invalid credentials" });
                }

                return NotFound(new { Message = "User not found" });
            }
            catch (Exception ex)
            {
                throw new GlobalException($"Error in AccountController: {ex}");
                return StatusCode(500, new { Message = "An error occurred during login" });
            }
        }
        [Authorize(Roles = "admin")]
        [HttpPut("AssignRole")]
        public async Task<ActionResult<AccountActionResponseDto>> AssignRole([FromBody] RoleDto assignRoleDto)
        {
           try{
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

           }catch(Exception ex)
           {
                return StatusCode(500, new { Message = "An error occurred during assigning a role to a user" });
           }
        }
        [Authorize(Roles ="admin")]
        [HttpPut("RemoveRole")]
        public async Task<IActionResult> RemoveRole([FromBody] RoleDto roleModel)
        {
          try{
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

            return BadRequest(removeRoleResponse);
          }
          catch (Exception ex)
          {
                return StatusCode(500, new { Message = "An error occurred during removing a user from role" });
        
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
            catch (Exception ex)
            {
            
            return StatusCode(500, new { Message = "An error occured when changing password" });

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
            catch (Exception ex)
            {
            return StatusCode(500, new { Message = "An error occurred when deleting userd" });

            }
        }

        [Authorize(Roles = "admin")]
        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            try 
            { 
                var allUsers = await userManager.Users.ToListAsync();

            if (allUsers != null)
            {
                 return Ok(allUsers);
            }

            return NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred when getting all users" });

            }
        }

        [HttpPut("UpdateProfile/{id:Guid}")]
        public async Task<ActionResult<AccountActionResponseDto>> UpdateUserProfile([FromBody] UpdateUserRequestDto updateUserRequestDto, [FromRoute] Guid id)
        {
            try
            {
                if (updateUserRequestDto == null)
                {
                    return BadRequest(new { Message = "Invalid user data" });
                }

                var user = await userManager.FindByIdAsync(id.ToString());
                if (user == null)
                {
                    return NotFound(new { Message = "User not found" });
                }

                user.Name = updateUserRequestDto.Name;
                user.LastName = updateUserRequestDto.LastName;
                user.PhoneNumber = updateUserRequestDto.PhoneNumber;
                user.Address = updateUserRequestDto.Address;

                var result = await userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    return BadRequest(new { Message = "Failed to update user profile", Errors = result.Errors });
                }
                var updateProfileResponse = new AccountActionResponseDto
                {
                    isSuccessful = result.Succeeded,
                    errors = result.Errors
                };
                return Ok(updateProfileResponse);

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while trying to update profile", Error = ex.Message });
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
            catch(Exception ex)
            {
                return StatusCode(500, new { Message="", Error= ex.Message  });

            }
        }   

    }


}