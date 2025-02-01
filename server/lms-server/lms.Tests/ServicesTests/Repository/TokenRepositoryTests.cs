using Xunit;
using Moq;
using lms.Abstractions.Interfaces;
using lms.Abstractions.Models;
using lms.Services.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace lms.Tests.ServicesTests.Repository
{
    public class TokenRepositoryTests
    {
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
        private readonly TokenRepository _repository;

        public TokenRepositoryTests()
        {
            _configurationMock = new Mock<IConfiguration>();
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
            _repository = new TokenRepository(_configurationMock.Object, _userManagerMock.Object);
        }

        [Fact]
        public async Task CreateJWTTokenAsync_ShouldReturnToken_WhenUserIsValid()
        {
            // Arrange
            var user = new ApplicationUser { UserName = "testuser", Id = Guid.NewGuid(), Email = "test@example.com" };
            _configurationMock.Setup(c => c["Jwt:Key"]).Returns("supersecretkey12345678901234567890");
            _configurationMock.Setup(c => c["Jwt:Issuer"]).Returns("testissuer");
            _configurationMock.Setup(c => c["Jwt:Audience"]).Returns("testaudience");
            _userManagerMock.Setup(um => um.GetRolesAsync(user)).ReturnsAsync(new List<string> { "User" });

            // Act
            var token = await _repository.CreateJWTTokenAsync(user);

            // Assert
            Assert.NotNull(token);
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            Assert.Equal("testissuer", jwtToken.Issuer);
            Assert.Equal("testaudience", jwtToken.Audiences.First());
            Assert.Equal(user.UserName, jwtToken.Claims.First(c => c.Type == JwtRegisteredClaimNames.Sub).Value);
        }

        [Fact]
        public async Task CreateJWTTokenAsync_ShouldThrowException_WhenConfigurationIsMissing()
        {
            // Arrange
            var user = new ApplicationUser { UserName = "testuser", Id = Guid.NewGuid(), Email = "test@example.com" };
            _configurationMock.Setup(c => c["Jwt:Key"]).Returns((string)null);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _repository.CreateJWTTokenAsync(user));
        }

        [Fact]
        public async Task CreateJWTTokenAsync_ShouldStoreToken_WhenUserIsValid()
        {
            // Arrange
            var user = new ApplicationUser { UserName = "testuser", Id = Guid.NewGuid(), Email = "test@example.com" };
            _configurationMock.Setup(c => c["Jwt:Key"]).Returns("supersecretkey12345678901234567890");
            _configurationMock.Setup(c => c["Jwt:Issuer"]).Returns("testissuer");
            _configurationMock.Setup(c => c["Jwt:Audience"]).Returns("testaudience");
            _userManagerMock.Setup(um => um.GetRolesAsync(user)).ReturnsAsync(new List<string> { "User" });

            // Act
            var token = await _repository.CreateJWTTokenAsync(user);

            // Assert
            Assert.NotNull(token);
            _userManagerMock.Verify(um => um.SetAuthenticationTokenAsync(user, "JWT", "AccessToken", token), Times.Once);
        }

        [Fact]
        public async Task CreateJWTTokenAsync_ShouldThrowException_WhenTokenStorageFails()
        {
            // Arrange
            var user = new ApplicationUser { UserName = "testuser", Id = Guid.NewGuid(), Email = "test@example.com" };
            _configurationMock.Setup(c => c["Jwt:Key"]).Returns("supersecretkey12345678901234567890");
            _configurationMock.Setup(c => c["Jwt:Issuer"]).Returns("testissuer");
            _configurationMock.Setup(c => c["Jwt:Audience"]).Returns("testaudience");
            _userManagerMock.Setup(um => um.GetRolesAsync(user)).ReturnsAsync(new List<string> { "User" });
            _userManagerMock.Setup(um => um.SetAuthenticationTokenAsync(user, "JWT", "AccessToken", It.IsAny<string>())).ThrowsAsync(new Exception("Token storage failed"));

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _repository.CreateJWTTokenAsync(user));
        }
    }
}