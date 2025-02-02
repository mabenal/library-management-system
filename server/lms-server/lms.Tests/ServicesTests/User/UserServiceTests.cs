using Xunit;
using Moq;
using lms.Abstractions.Interfaces;
using lms.Abstractions.Models;
using lms.Services;
using Microsoft.AspNetCore.Identity;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace lms.Tests.ServicesTests.User
{
    public class UserServiceTests
    {
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
        private readonly Mock<IClientRepository> _clientRepositoryMock;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
            _clientRepositoryMock = new Mock<IClientRepository>();
            _userService = new UserService(_userManagerMock.Object, _clientRepositoryMock.Object);
        }

        [Fact]
        public async Task GetUserIdAsync_ReturnsNull_WhenUserEmailIsNull()
        {
            // Arrange
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>()));

            // Act
            var result = await _userService.GetUserIdAsync(claimsPrincipal);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetUserIdAsync_ReturnsNull_WhenClientNotFound()
        {
            // Arrange
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, "test@example.com") };
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims));

            _clientRepositoryMock.Setup(repo => repo.GetClientbyEmail("test@example.com"))
                .ReturnsAsync((Client)null);

            // Act
            var result = await _userService.GetUserIdAsync(claimsPrincipal);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetUserIdAsync_ReturnsClientId_WhenClientFound()
        {
            // Arrange
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, "test@example.com") };
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims));
            var client = new Client { Id = Guid.NewGuid(), EmailAddress = "test@example.com" };

            _clientRepositoryMock.Setup(repo => repo.GetClientbyEmail("test@example.com"))
                .ReturnsAsync(client);

            // Act
            var result = await _userService.GetUserIdAsync(claimsPrincipal);

            // Assert
            Assert.Equal(client.Id, result);
        }
    }
}