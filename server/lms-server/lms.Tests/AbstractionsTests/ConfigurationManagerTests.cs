using Xunit;
using Moq;
using Microsoft.Extensions.Configuration;
using lms.Abstractions;
using System;

namespace lms.Tests.AbstractionsTests
{
    public class ConfigurationManagerTests
    {
        [Fact]
        public void GetInstance_ShouldReturnSameInstance()
        {
            // Arrange
            var configurationMock = new Mock<IConfiguration>();

            // Act
            var instance1 = Abstractions.ConfigurationManager.GetInstance(configurationMock.Object);
            var instance2 = Abstractions.ConfigurationManager.GetInstance(configurationMock.Object);

            // Assert
            Assert.Same(instance1, instance2);
        }

        [Fact]
        public void GetJwtKey_ShouldReturnKey_WhenConfigurationIsSet()
        {
            // Arrange
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(c => c["Jwt:Key"]).Returns("test-key");
            var configManager = Abstractions.ConfigurationManager.GetInstance(configurationMock.Object);

            // Act
            var key = configManager.GetJwtKey();

            // Assert
            Assert.Equal("test-key", key);
        }

        [Fact]
        public void GetJwtKey_ShouldThrowException_WhenConfigurationIsNotSet()
        {
            // Arrange
            var configurationMock = new Mock<IConfiguration>();
            var configManager = Abstractions.ConfigurationManager.GetInstance(configurationMock.Object);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => configManager.GetJwtKey());
        }

        [Fact]
        public void GetJwtIssuer_ShouldReturnIssuer_WhenConfigurationIsSet()
        {
            // Arrange
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(c => c["Jwt:Issuer"]).Returns("test-issuer");
            var configManager = Abstractions.ConfigurationManager.GetInstance(configurationMock.Object);

            // Act
            var issuer = configManager.GetJwtIssuer();

            // Assert
            Assert.Equal("test-issuer", issuer);
        }

        [Fact]
        public void GetJwtIssuer_ShouldThrowException_WhenConfigurationIsNotSet()
        {
            // Arrange
            var configurationMock = new Mock<IConfiguration>();
            var configManager = Abstractions.ConfigurationManager.GetInstance(configurationMock.Object);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => configManager.GetJwtIssuer());
        }

        [Fact]
        public void GetJwtAudience_ShouldReturnAudience_WhenConfigurationIsSet()
        {
            // Arrange
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(c => c["Jwt:Audience"]).Returns("test-audience");
            var configManager = Abstractions.ConfigurationManager.GetInstance(configurationMock.Object);

            // Act
            var audience = configManager.GetJwtAudience();

            // Assert
            Assert.Equal("test-audience", audience);
        }

        [Fact]
        public void GetJwtAudience_ShouldThrowException_WhenConfigurationIsNotSet()
        {
            // Arrange
            var configurationMock = new Mock<IConfiguration>();
            var configManager = Abstractions.ConfigurationManager.GetInstance(configurationMock.Object);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => configManager.GetJwtAudience());
        }
    }
}