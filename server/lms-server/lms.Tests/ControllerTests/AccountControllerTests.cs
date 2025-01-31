using lms.Abstractions.Interfaces;
using lms.Abstractions.Models;
using lms.Abstractions.Models.DTO;
using lms.Abstractions.Exceptions;
using lms.Peer.Controllers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using MockQueryable;

namespace lms.Tests.ControllerTests
{
    public class AccountControllerTests
    {
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
        private readonly Mock<SignInManager<ApplicationUser>> _signInManagerMock;
        private readonly Mock<ITokenRepository> _tokenRepositoryMock;
        private readonly AccountController _controller;

        public AccountControllerTests()
        {
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
            var contextAccessorMock = new Mock<IHttpContextAccessor>();
            var userClaimsPrincipalFactoryMock = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();
            _signInManagerMock = new Mock<SignInManager<ApplicationUser>>(_userManagerMock.Object, contextAccessorMock.Object, userClaimsPrincipalFactoryMock.Object, null, null, null, null);
            _tokenRepositoryMock = new Mock<ITokenRepository>();

            _controller = new AccountController(_userManagerMock.Object, _signInManagerMock.Object, _tokenRepositoryMock.Object);
        }

        [Fact]
        public async Task RegisterNewUser_ReturnsOk_WhenRegistrationIsSuccessful()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                Email = "test@example.com",
                Password = "Password123!",
                ConfirmPassword = "Password123!",
                Name = "Test",
                LastName = "User",
                Address = "123 Test St",
                PhoneNumber = "123-456-7890"
            };

            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _controller.RegisterNewUser(registerDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<RegisterResponseDto>(okResult.Value);
            Assert.True(response.Succeeded);
        }

        [Fact]
        public async Task RegisterNewUser_ReturnsBadRequest_WhenRegistrationFails()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                Email = "test@example.com",
                Password = "Password123!",
                ConfirmPassword = "Password123!",
                Name = "Test",
                LastName = "User",
                Address = "123 Test St",
                PhoneNumber = "123-456-7890"
            };

            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Registration failed" }));

            // Act
            var result = await _controller.RegisterNewUser(registerDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var errors = Assert.IsType<List<IdentityError>>(badRequestResult.Value);
            Assert.Single(errors);
            Assert.Equal("Registration failed", errors[0].Description);
        }

        [Fact]
        public async Task RegisterNewUser_ThrowsGlobalException()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                Email = "test@example.com",
                Password = "Password123!",
                ConfirmPassword = "Password123!",
                Name = "Test",
                LastName = "User",
                Address = "123 Test St",
                PhoneNumber = "123-456-7890"
            };

            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception("Test exception"));

            // Act & Assert
            await Assert.ThrowsAsync<GlobalException>(() => _controller.RegisterNewUser(registerDto));
        }

        [Fact]
        public async Task Login_ReturnsOk_WhenLoginIsSuccessful()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                UserName = "test@example.com",
                Password = "Password123!"
            };

            var user = new ApplicationUser { UserName = loginDto.UserName };

            _userManagerMock.Setup(x => x.FindByNameAsync(loginDto.UserName))
                .ReturnsAsync(user);

            _signInManagerMock.Setup(x => x.PasswordSignInAsync(user, loginDto.Password, false, false))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

            _tokenRepositoryMock.Setup(x => x.CreateJWTTokenAsync(user))
                .ReturnsAsync("fake-jwt-token");

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<LoginResponseDto>(okResult.Value);
            Assert.Equal("fake-jwt-token", response.Token);
            Assert.Equal(loginDto.UserName, response.Username);
        }

        //[Fact]
        //public async Task Login_ReturnsNotFound_WhenUserNotFound()
        //{
        //    // Arrange
        //    var loginDto = new LoginDto
        //    {
        //        UserName = "test@example.com",
        //        Password = "Password123!"
        //    };

        //    _userManagerMock.Setup(x => x.FindByNameAsync(loginDto.UserName))
        //        .ReturnsAsync((ApplicationUser)null);

        //    // Act
        //    var result = await _controller.Login(loginDto);

        //    // Assert
        //    var actionResult = Assert.IsType<ActionResult<LoginResponseDto>>(result);
        //    var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
        //    Assert.Equal("User not found", notFoundResult.Value);
        //}

        //[Fact]
        //public async Task Login_ReturnsBadRequest_WhenInvalidCredentials()
        //{
        //    // Arrange
        //    var loginDto = new LoginDto
        //    {
        //        UserName = "test@example.com",
        //        Password = "Password123!"
        //    };

        //    var user = new ApplicationUser { UserName = loginDto.UserName };

        //    _userManagerMock.Setup(x => x.FindByNameAsync(loginDto.UserName))
        //        .ReturnsAsync(user);

        //    _signInManagerMock.Setup(x => x.PasswordSignInAsync(user, loginDto.Password, false, false))
        //        .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed);

        //    // Act
        //    var result = await _controller.Login(loginDto);

        //    // Assert
        //    var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        //    var actualValue = badRequestResult.Value as IDictionary<string, object>;
        //    Assert.Equal("Invalid credentials", actualValue["Message"]);
        //}

        [Fact]
        public async Task Login_ThrowsGlobalException()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                UserName = "test@example.com",
                Password = "Password123!"
            };

            _userManagerMock.Setup(x => x.FindByNameAsync(loginDto.UserName))
                .ThrowsAsync(new Exception("Test exception"));

            // Act & Assert
            await Assert.ThrowsAsync<GlobalException>(() => _controller.Login(loginDto));
        }

        [Fact]
        public async Task AssignRole_ReturnsOk_WhenRoleAssignmentIsSuccessful()
        {
            // Arrange
            var roleDto = new RoleDto
            {
                UserId = Guid.NewGuid(),
                Role = "admin"
            };

            var user = new ApplicationUser { Id = roleDto.UserId };

            _userManagerMock.Setup(x => x.FindByIdAsync(roleDto.UserId.ToString()))
                .ReturnsAsync(user);

            _userManagerMock.Setup(x => x.AddToRoleAsync(user, roleDto.Role))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _controller.AssignRole(roleDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<AccountActionResponseDto>(okResult.Value);
            Assert.True(response.isSuccessful);
        }

        //[Fact]
        //public async Task AssignRole_ReturnsNotFound_WhenUserNotFound()
        //{
        //    // Arrange
        //    var roleDto = new RoleDto
        //    {
        //        UserId = Guid.NewGuid(),
        //        Role = "admin"
        //    };

        //    _userManagerMock.Setup(x => x.FindByIdAsync(roleDto.UserId.ToString()))
        //        .ReturnsAsync((ApplicationUser)null);

        //    // Act
        //    var result = await _controller.AssignRole(roleDto);

        //    // Assert
        //    var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        //    Assert.Equal("{ Message = \"User not found\" }", notFoundResult.Value);
        //}

        [Fact]
        public async Task AssignRole_ThrowsGlobalException()
        {
            // Arrange
            var roleDto = new RoleDto
            {
                UserId = Guid.NewGuid(),
                Role = "admin"
            };

            _userManagerMock.Setup(x => x.FindByIdAsync(roleDto.UserId.ToString()))
                .ThrowsAsync(new GlobalException("Test exception"));

            // Act & Assert
            await Assert.ThrowsAsync<GlobalException>(() => _controller.AssignRole(roleDto));
        }

        [Fact]
        public async Task RemoveRole_ReturnsOk_WhenRoleRemovalIsSuccessful()
        {
            // Arrange
            var roleDto = new RoleDto
            {
                UserId = Guid.NewGuid(),
                Role = "admin"
            };

            var user = new ApplicationUser { Id = roleDto.UserId };

            _userManagerMock.Setup(x => x.FindByIdAsync(roleDto.UserId.ToString()))
                .ReturnsAsync(user);

            _userManagerMock.Setup(x => x.RemoveFromRoleAsync(user, roleDto.Role))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _controller.RemoveRole(roleDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<AccountActionResponseDto>(okResult.Value);
            Assert.True(response.isSuccessful);
        }

        //[Fact]
        //public async Task RemoveRole_ReturnsNotFound_WhenUserNotFound()
        //{
        //    // Arrange
        //    var roleDto = new RoleDto
        //    {
        //        UserId = Guid.NewGuid(),
        //        Role = "admin"
        //    };

        //    _userManagerMock.Setup(x => x.FindByIdAsync(roleDto.UserId.ToString()))
        //        .ReturnsAsync((ApplicationUser)null);

        //    // Act
        //    var result = await _controller.RemoveRole(roleDto);

        //    // Assert
        //    var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        //    Assert.Equal("User not found", notFoundResult.Value);
        //}

        [Fact]
        public async Task RemoveRole_ThrowsGlobalException()
        {
            // Arrange
            var roleDto = new RoleDto
            {
                UserId = Guid.NewGuid(),
                Role = "admin"
            };

            _userManagerMock.Setup(x => x.FindByIdAsync(roleDto.UserId.ToString()))
                .ThrowsAsync(new GlobalException("Test exception"));

            // Act & Assert
            await Assert.ThrowsAsync<GlobalException>(() => _controller.RemoveRole(roleDto));
        }

        [Fact]
        public async Task ChangePassword_ReturnsOk_WhenPasswordChangeIsSuccessful()
        {
            // Arrange
            var changePasswordDto = new ChangePasswordRequestDto
            {
                Username = "test@example.com",
                CurrentPassword = "OldPassword123!",
                NewPassword = "NewPassword123!"
            };

            var user = new ApplicationUser { UserName = changePasswordDto.Username };

            _userManagerMock.Setup(x => x.FindByNameAsync(changePasswordDto.Username))
                .ReturnsAsync(user);

            _userManagerMock.Setup(x => x.ChangePasswordAsync(user, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _controller.ChangePassword(changePasswordDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<AccountActionResponseDto>(okResult.Value);
            Assert.True(response.isSuccessful);
        }

        [Fact]
        public async Task ChangePassword_ReturnsNotFound_WhenUserNotFound()
        {
            // Arrange
            var changePasswordDto = new ChangePasswordRequestDto
            {
                Username = "test@example.com",
                CurrentPassword = "OldPassword123!",
                NewPassword = "NewPassword123!"
            };

            _userManagerMock.Setup(x => x.FindByNameAsync(changePasswordDto.Username))
                .ReturnsAsync((ApplicationUser)null);

            // Act
            var result = await _controller.ChangePassword(changePasswordDto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task ChangePassword_ReturnsBadRequest_WhenPasswordChangeFails()
        {
            // Arrange
            var changePasswordDto = new ChangePasswordRequestDto
            {
                Username = "test@example.com",
                CurrentPassword = "OldPassword123!",
                NewPassword = "NewPassword123!"
            };

            var user = new ApplicationUser { UserName = changePasswordDto.Username };

            _userManagerMock.Setup(x => x.FindByNameAsync(changePasswordDto.Username))
                .ReturnsAsync(user);

            var identityResult = IdentityResult.Failed(new IdentityError { Description = "Password change failed" });
            _userManagerMock.Setup(x => x.ChangePasswordAsync(user, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword))
                .ReturnsAsync(identityResult);

            // Act
            var result = await _controller.ChangePassword(changePasswordDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var errors = Assert.IsType<IdentityResult>(badRequestResult.Value).Errors;
            Assert.Single(errors);
            Assert.Equal("Password change failed", errors.First().Description);
        }

        [Fact]
        public async Task ChangePassword_ThrowsGlobalException()
        {
            // Arrange
            var changePasswordDto = new ChangePasswordRequestDto
            {
                Username = "test@example.com",
                CurrentPassword = "OldPassword123!",
                NewPassword = "NewPassword123!"
            };

            _userManagerMock.Setup(x => x.FindByNameAsync(changePasswordDto.Username))
                .ThrowsAsync(new Exception("Test exception"));

            // Act & Assert
            await Assert.ThrowsAsync<GlobalException>(() => _controller.ChangePassword(changePasswordDto));
        }

        [Fact]
        public async Task DeleteUser_ReturnsOk_WhenUserDeletionIsSuccessful()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new ApplicationUser { Id = userId };

            _userManagerMock.Setup(x => x.FindByIdAsync(userId.ToString()))
                .ReturnsAsync(user);

            _userManagerMock.Setup(x => x.DeleteAsync(user))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _controller.DeleteUser(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<AccountActionResponseDto>(okResult.Value);
            Assert.True(response.isSuccessful);
        }

        [Fact]
        public async Task DeleteUser_ReturnsNotFound_WhenUserNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();

            _userManagerMock.Setup(x => x.FindByIdAsync(userId.ToString()))
                .ReturnsAsync((ApplicationUser)null);

            // Act
            var result = await _controller.DeleteUser(userId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task DeleteUser_ThrowsGlobalException()
        {
            // Arrange
            var userId = Guid.NewGuid();

            _userManagerMock.Setup(x => x.FindByIdAsync(userId.ToString()))
                .ThrowsAsync(new Exception("Test exception"));

            // Act & Assert
            await Assert.ThrowsAsync<GlobalException>(() => _controller.DeleteUser(userId));
        }

        [Fact]
        public async Task GetAllUsers_ReturnsOk_WhenUsersExist()
        {
            // Arrange
            var users = new List<ApplicationUser>
            {
                new ApplicationUser { UserName = "user1@example.com" },
                new ApplicationUser { UserName = "user2@example.com" }
            }.AsQueryable().BuildMock();

            _userManagerMock.Setup(x => x.Users).Returns(users);

            // Act
            var result = await _controller.GetAllUsers();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<List<ApplicationUser>>(okResult.Value);
            Assert.Equal(2, response.Count);
        }

        [Fact]
        public async Task GetAllUsers_ReturnsNotFound_WhenNoUsersExist()
        {
            // Arrange
            var users = new List<ApplicationUser>().AsQueryable().BuildMock();

            _userManagerMock.Setup(x => x.Users).Returns(users);

            // Act
            var result = await _controller.GetAllUsers();

            // Assert
            var notFoundResult = Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetAllUsers_ThrowsGlobalException()
        {
            // Arrange
            _userManagerMock.Setup(x => x.Users).Throws(new Exception("Test exception"));

            // Act & Assert
            await Assert.ThrowsAsync<GlobalException>(() => _controller.GetAllUsers());
        }
    }
}