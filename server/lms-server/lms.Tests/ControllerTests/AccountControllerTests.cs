using lms.Abstractions.Exceptions;
using lms.Abstractions.Interfaces;
using lms.Abstractions.Models;
using lms.Abstractions.Models.DTO;
using lms.Peer.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MockQueryable;
using Moq;

namespace lms.Tests.ControllerTests
{
    public class AccountControllerTests
    {
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
        private readonly Mock<SignInManager<ApplicationUser>> _signInManagerMock;
        private readonly Mock<ITokenRepository> _tokenRepositoryMock;
        private readonly Mock<ILmsDbContext> _contextMock;
        private readonly AccountController _controller;

        public AccountControllerTests()
        {
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
            var contextAccessorMock = new Mock<IHttpContextAccessor>();
            var userClaimsPrincipalFactoryMock = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();
            _signInManagerMock = new Mock<SignInManager<ApplicationUser>>(_userManagerMock.Object, contextAccessorMock.Object, userClaimsPrincipalFactoryMock.Object, null, null, null, null);
            _tokenRepositoryMock = new Mock<ITokenRepository>();
            _contextMock = new Mock<ILmsDbContext>();

            _controller = new AccountController(_userManagerMock.Object, _signInManagerMock.Object, _tokenRepositoryMock.Object, _contextMock.Object);
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

            _userManagerMock.Setup(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), "client"))
                .ReturnsAsync(IdentityResult.Success);

            _contextMock.Setup(x => x.Clients.AddAsync(It.IsAny<Client>(), It.IsAny<CancellationToken>()))
                .Returns((Client client, CancellationToken token) => new ValueTask<EntityEntry<Client>>(Task.FromResult((EntityEntry<Client>)null)));

            _contextMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

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
                .ThrowsAsync(new GlobalException("Test exception"));

            // Act & Assert
            await Assert.ThrowsAsync<GlobalException>(() => _controller.RegisterNewUser(registerDto));
        }
        
        [Fact]
        public async Task RegisterNewUser_ReturnsInternalServerError_WhenExceptionOccurs()
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

            // Act
            var result = await _controller.RegisterNewUser(registerDto);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);
            var problemDetails = Assert.IsType<ProblemDetails>(statusCodeResult.Value);
            Assert.Equal("An internal server error occurred. Please try again later.", problemDetails.Title);
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

            _userManagerMock.Setup(x => x.GetRolesAsync(user))
                .ReturnsAsync(new List<string> { "admin" });

            _tokenRepositoryMock.Setup(x => x.CreateJWTTokenAsync(user))
                .ReturnsAsync("fake-jwt-token");

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<LoginResponseDto>(okResult.Value);
            Assert.Equal("fake-jwt-token", response.Token);
            Assert.Equal(loginDto.UserName, response.Username);
            Assert.Contains("admin", response.UserRoles);
        }

        [Fact]
        public async Task Login_ReturnsNotFound_WhenUserNotFound()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                UserName = "test@example.com",
                Password = "Password123!"
            };

            _userManagerMock.Setup(x => x.FindByNameAsync(loginDto.UserName))
                .ReturnsAsync((ApplicationUser)null);

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            var actionResult = Assert.IsType<ActionResult<LoginResponseDto>>(result);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
            var problemDetails = Assert.IsType<ProblemDetails>(notFoundResult.Value);
            Assert.Equal("An internal server error occurred. Please try again later.", problemDetails.Title);
        }

        [Fact]
        public async Task Login_ReturnsBadRequest_WhenInvalidCredentials()
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
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed);

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var problemDetails = Assert.IsType<ProblemDetails>(badRequestResult.Value);
            Assert.Equal("An internal server error occurred. Please try again later.", problemDetails.Title);
        }

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
                .ThrowsAsync(new GlobalException("Test exception"));

            // Act & Assert
            await Assert.ThrowsAsync<GlobalException>(() => _controller.Login(loginDto));
        }

        [Fact]
        public async Task Login_ReturnsInternalServerError_WhenExceptionOccurs()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                UserName = "test@example.com",
                Password = "Password123!"
            };

            _userManagerMock.Setup(x => x.FindByNameAsync(loginDto.UserName))
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);
            var problemDetails = Assert.IsType<ProblemDetails>(statusCodeResult.Value);
            Assert.Equal("An internal server error occurred. Please try again later.", problemDetails.Title);
        }

        [Fact]
        public async Task AssignRole_ReturnsNotFound_WhenUserNotFound()
        {
            // Arrange
            Guid userId = Guid.NewGuid();
            string? role = "admin";

            _userManagerMock.Setup(x => x.FindByIdAsync(userId.ToString()))
                .ReturnsAsync((ApplicationUser)null);

            // Act
            var result = await _controller.AssignRole(userId, role);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            var problemDetails = Assert.IsType<ProblemDetails>(notFoundResult.Value);
            Assert.Equal("User not found", problemDetails.Title);
        }

        [Fact]
        public async Task AssignRole_ThrowsGlobalException_WhenGlobalExceptionOccurs()
        {
            // Arrange
            Guid userId = Guid.NewGuid();
            string role = "admin";

            _userManagerMock.Setup(x => x.FindByIdAsync(userId.ToString()))
                .ThrowsAsync(new GlobalException("Test GlobalException"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<GlobalException>(() => _controller.AssignRole(userId, role));
            Assert.Contains("in accountController:", exception.Message);
            Assert.Contains("Test GlobalException", exception.Message);
        }

        [Fact]
        public async Task AssignRole_RemovesClient_WhenRoleIsNotClient()
        {
            // Arrange
            Guid userId = Guid.NewGuid();
            string? role = "admin";

            var user = new ApplicationUser { Id = userId };
            var client = new Client { Id = user.Id };

            _userManagerMock.Setup(x => x.FindByIdAsync(userId.ToString()))
                .ReturnsAsync(user);

            _userManagerMock.Setup(x => x.GetRolesAsync(user))
                .ReturnsAsync(new List<string> { "Client" });

            _userManagerMock.Setup(x => x.RemoveFromRolesAsync(user, It.IsAny<string[]>()))
                .ReturnsAsync(IdentityResult.Success);

            _userManagerMock.Setup(x => x.AddToRoleAsync(user, role))
                .ReturnsAsync(IdentityResult.Success);

            _contextMock.Setup(x => x.Clients.FindAsync(user.Id))
                .ReturnsAsync(client);

            _contextMock.Setup(x => x.Clients.Remove(client));
            _contextMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _controller.AssignRole(userId, role);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<AccountActionResponseDto>(okResult.Value);
            Assert.True(response.isSuccessful);
            _contextMock.Verify(x => x.Clients.Remove(client), Times.Once);
            _contextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task AssignRole_ReturnsInternalServerError_WhenExceptionOccurs()
        {
            // Arrange
            Guid userId = Guid.NewGuid();
            string? role = "admin";

            _userManagerMock.Setup(x => x.FindByIdAsync(userId.ToString()))
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.AssignRole(userId, role);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);
            var problemDetails = Assert.IsType<ProblemDetails>(statusCodeResult.Value);
            Assert.Equal("An internal server error occurred. Please try again later.", problemDetails.Title);
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
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<AccountActionResponseDto>(okResult.Value);
            Assert.True(response.isSuccessful);
        }

        [Fact]
        public async Task RemoveRole_ReturnsNotFound_WhenUserNotFound()
        {
            // Arrange
            var roleDto = new RoleDto
            {
                UserId = Guid.NewGuid(),
                Role = "admin"
            };

            _userManagerMock.Setup(x => x.FindByIdAsync(roleDto.UserId.ToString()))
                .ReturnsAsync((ApplicationUser)null);

            // Act
            var result = await _controller.RemoveRole(roleDto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            var problemDetails = Assert.IsType<ProblemDetails>(notFoundResult.Value);
            Assert.Equal("User not found", problemDetails.Title);
        }

        [Fact]
        public async Task RemoveRole_ReturnsBadRequest_WhenRoleRemovalFails()
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

            var identityResult = IdentityResult.Failed(new IdentityError { Description = "Role removal failed" });
            _userManagerMock.Setup(x => x.RemoveFromRoleAsync(user, roleDto.Role))
                .ReturnsAsync(identityResult);

            // Act
            var result = await _controller.RemoveRole(roleDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var response = Assert.IsType<AccountActionResponseDto>(badRequestResult.Value);
            Assert.False(response.isSuccessful);
            Assert.Single(response.errors);
            Assert.Equal("Role removal failed", response.errors.First().Description);
        }

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
        public async Task RemoveRole_ReturnsInternalServerError_WhenExceptionOccurs()
        {
            // Arrange
            var roleDto = new RoleDto
            {
                UserId = Guid.NewGuid(),
                Role = "admin"
            };

            _userManagerMock.Setup(x => x.FindByIdAsync(roleDto.UserId.ToString()))
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.RemoveRole(roleDto);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);
            var problemDetails = Assert.IsType<ProblemDetails>(statusCodeResult.Value);
            Assert.Equal("An internal server error occurred. Please try again later.", problemDetails.Title);
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
        public async Task ChangePassword_ReturnsBadRequest_WhenChangePasswordFails()
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

            var identityResult = IdentityResult.Failed(new IdentityError { Description = "Change password failed" });
            _userManagerMock.Setup(x => x.ChangePasswordAsync(user, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword))
                .ReturnsAsync(identityResult);

            // Act
            var result = await _controller.ChangePassword(changePasswordDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var errors = Assert.IsType<IdentityResult>(badRequestResult.Value).Errors;
            Assert.Single(errors);
            Assert.Equal("Change password failed", errors.First().Description);
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
                .ThrowsAsync(new GlobalException("Test exception"));

            // Act & Assert
            await Assert.ThrowsAsync<GlobalException>(() => _controller.ChangePassword(changePasswordDto));
        }

        [Fact]
        public async Task ChangePassword_ReturnsInternalServerError_WhenExceptionOccurs()
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

            // Act
            var result = await _controller.ChangePassword(changePasswordDto);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);
            var problemDetails = Assert.IsType<ProblemDetails>(statusCodeResult.Value);
            Assert.Equal("An error occured when changing password", problemDetails.Title);
        }

        [Fact]
        public async Task DeleteUser_RemovesClient_WhenClientExists()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new ApplicationUser { Id = userId };
            var client = new Client { Id = userId };
            var deleteResponse = new AccountActionResponseDto { isSuccessful = true };

            _userManagerMock.Setup(x => x.FindByIdAsync(userId.ToString()))
                .ReturnsAsync(user);

            _userManagerMock.Setup(x => x.DeleteAsync(user))
                .ReturnsAsync(IdentityResult.Success);

            _contextMock.Setup(x => x.Clients.FindAsync(user.Id))
                .ReturnsAsync(client);

            _contextMock.Setup(x => x.Clients.Remove(client));
            _contextMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _controller.DeleteUser(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<AccountActionResponseDto>(okResult.Value);
            Assert.True(response.isSuccessful);
            _contextMock.Verify(x => x.Clients.Remove(client), Times.Once);
            _contextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteUser_DoesNotRemoveClient_WhenClientDoesNotExist()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new ApplicationUser { Id = userId };
            Client client = null;
            var deleteResponse = new AccountActionResponseDto { isSuccessful = true };

            _userManagerMock.Setup(x => x.FindByIdAsync(userId.ToString()))
                .ReturnsAsync(user);

            _userManagerMock.Setup(x => x.DeleteAsync(user))
                .ReturnsAsync(IdentityResult.Success);

            _contextMock.Setup(x => x.Clients.FindAsync(user.Id))
                .ReturnsAsync(client);

            // Act
            var result = await _controller.DeleteUser(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<AccountActionResponseDto>(okResult.Value);
            Assert.True(response.isSuccessful);
            _contextMock.Verify(x => x.Clients.Remove(It.IsAny<Client>()), Times.Never);
            _contextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task DeleteUser_ReturnsBadRequest_WhenDeleteFails()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new ApplicationUser { Id = userId };

            _userManagerMock.Setup(x => x.FindByIdAsync(userId.ToString()))
                .ReturnsAsync(user);

            var identityResult = IdentityResult.Failed(new IdentityError { Description = "Delete failed" });
            _userManagerMock.Setup(x => x.DeleteAsync(user))
                .ReturnsAsync(identityResult);

            // Act
            var result = await _controller.DeleteUser(userId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var response = Assert.IsType<AccountActionResponseDto>(badRequestResult.Value);
            Assert.False(response.isSuccessful);
            Assert.Single(response.errors);
            Assert.Equal("Delete failed", response.errors.First().Description);
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
                .ThrowsAsync(new GlobalException("Test exception"));

            // Act & Assert
            await Assert.ThrowsAsync<GlobalException>(() => _controller.DeleteUser(userId));
        }

        [Fact]
        public async Task DeleteUser_ReturnsInternalServerError_WhenExceptionOccurs()
        {
            // Arrange
            var userId = Guid.NewGuid();

            _userManagerMock.Setup(x => x.FindByIdAsync(userId.ToString()))
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.DeleteUser(userId);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);
            var problemDetails = Assert.IsType<ProblemDetails>(statusCodeResult.Value);
            Assert.Equal("An error occurred when deleting user", problemDetails.Title);
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
            _userManagerMock.Setup(x => x.Users).Throws(new GlobalException("Test exception"));

            // Act & Assert
            await Assert.ThrowsAsync<GlobalException>(() => _controller.GetAllUsers());
        }

        [Fact]
        public async Task GetAllUsers_ReturnsInternalServerError_WhenExceptionOccurs()
        {
            // Arrange
            _userManagerMock.Setup(x => x.Users).Throws(new Exception("Test exception"));

            // Act
            var result = await _controller.GetAllUsers();

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);
            var problemDetails = Assert.IsType<ProblemDetails>(statusCodeResult.Value);
            Assert.Equal("An error occurred when getting all users", problemDetails.Title);
        }

        [Fact]
        public async Task GetUserProfile_ReturnsOk_WhenUserExists()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new ApplicationUser { Id = userId };

            _userManagerMock.Setup(x => x.FindByIdAsync(userId.ToString()))
                .ReturnsAsync(user);

            // Act
            var result = await _controller.GetUserProfile(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<ApplicationUser>(okResult.Value);
            Assert.Equal(user, response);
        }

        [Fact]
        public async Task GetUserProfile_ReturnsNotFound_WhenUserNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();

            _userManagerMock.Setup(x => x.FindByIdAsync(userId.ToString()))
                .ReturnsAsync((ApplicationUser)null);

            // Act
            var result = await _controller.GetUserProfile(userId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task UpdateUserProfile_ReturnsOk_WhenUpdateIsSuccessful()
        {
            // Arrange
            var updateUserRequestDto = new UpdateUserRequestDto
            {
                Name = "Test",
                LastName = "User",
                Address = "123 Test St",
                PhoneNumber = "1234567890"
            };
            var userId = Guid.NewGuid();
            var user = new ApplicationUser { Id = userId };

            _userManagerMock.Setup(x => x.FindByIdAsync(userId.ToString()))
                .ReturnsAsync(user);

            _userManagerMock.Setup(x => x.UpdateAsync(user))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _controller.UpdateUserProfile(updateUserRequestDto, userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<AccountActionResponseDto>(okResult.Value);
            Assert.True(response.isSuccessful);
        }

        [Fact]
        public async Task UpdateUserProfile_ReturnsBadRequest_WhenUpdateFails()
        {
            // Arrange
            var updateUserRequestDto = new UpdateUserRequestDto
            {
                Name = "Test",
                LastName = "User",
                Address = "123 Test St",
                PhoneNumber = "1234567890"
            };
            var userId = Guid.NewGuid();
            var user = new ApplicationUser { Id = userId };

            _userManagerMock.Setup(x => x.FindByIdAsync(userId.ToString()))
                .ReturnsAsync(user);

            var identityResult = IdentityResult.Failed(new IdentityError { Description = "Update failed" });
            _userManagerMock.Setup(x => x.UpdateAsync(user))
                .ReturnsAsync(identityResult);

            // Act
            var result = await _controller.UpdateUserProfile(updateUserRequestDto, userId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var problemDetails = Assert.IsType<ProblemDetails>(badRequestResult.Value);
            Assert.Equal("Failed to update user profile", problemDetails.Title);
        }

        [Fact]
        public async Task UpdateUserProfile_ReturnsNotFound_WhenUserNotFound()
        {
            // Arrange
            var updateUserRequestDto = new UpdateUserRequestDto
            {
                Name = "Test",
                LastName = "User",
                Address = "123 Test St",
                PhoneNumber = "1234567890"
            };
            var userId = Guid.NewGuid();

            _userManagerMock.Setup(x => x.FindByIdAsync(userId.ToString()))
                .ReturnsAsync((ApplicationUser)null);

            // Act
            var result = await _controller.UpdateUserProfile(updateUserRequestDto, userId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            var problemDetails = Assert.IsType<ProblemDetails>(notFoundResult.Value);
            Assert.Equal("User not found", problemDetails.Title);
        }

        [Fact]
        public async Task UpdateUserProfile_ThrowsGlobalException()
        {
            // Arrange
            var updateUserRequestDto = new UpdateUserRequestDto
            {
                Name = "Test",
                LastName = "User",
                Address = "123 Test St",
                PhoneNumber = "1234567890"
            };
            var userId = Guid.NewGuid();

            _userManagerMock.Setup(x => x.FindByIdAsync(userId.ToString()))
                .ThrowsAsync(new GlobalException("Test exception"));

            // Act & Assert
            await Assert.ThrowsAsync<GlobalException>(() => _controller.UpdateUserProfile(updateUserRequestDto, userId));
        }

        [Fact]
        public async Task UpdateUserProfile_ReturnsBadRequest_WhenUserDataIsInvalid()
        {
            // Arrange
            UpdateUserRequestDto updateUserRequestDto = null;
            var userId = Guid.NewGuid();

            // Act
            var result = await _controller.UpdateUserProfile(updateUserRequestDto, userId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var problemDetails = Assert.IsType<ProblemDetails>(badRequestResult.Value);
            Assert.Equal("Invalid user data", problemDetails.Title);
        }

        [Fact]
        public async Task UpdateUserProfile_ReturnsInternalServerError_WhenExceptionOccurs()
        {
            // Arrange
            var updateUserRequestDto = new UpdateUserRequestDto
            {
                Name = "Test",
                LastName = "User",
                Address = "123 Test St",
                PhoneNumber = "1234567890"
            };
            var userId = Guid.NewGuid();

            _userManagerMock.Setup(x => x.FindByIdAsync(userId.ToString()))
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.UpdateUserProfile(updateUserRequestDto, userId);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);
            var problemDetails = Assert.IsType<ProblemDetails>(statusCodeResult.Value);
            Assert.Equal("An error occurred when updating user profile", problemDetails.Title);
        }

        [Fact]
        public async Task GetUserProfile_ThrowsGlobalException()
        {
            // Arrange
            var userId = Guid.NewGuid();

            _userManagerMock.Setup(x => x.FindByIdAsync(userId.ToString()))
                .ThrowsAsync(new GlobalException("Test exception"));

            // Act & Assert
            await Assert.ThrowsAsync<GlobalException>(() => _controller.GetUserProfile(userId));
        }

        [Fact]
        public async Task GetUserProfile_ReturnsInternalServerError_WhenExceptionOccurs()
        {
            // Arrange
            var userId = Guid.NewGuid();

            _userManagerMock.Setup(x => x.FindByIdAsync(userId.ToString()))
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.GetUserProfile(userId);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);
            var problemDetails = Assert.IsType<ProblemDetails>(statusCodeResult.Value);
            Assert.Equal("An error occurred when getting user profile", problemDetails.Title);
        }
    }
}