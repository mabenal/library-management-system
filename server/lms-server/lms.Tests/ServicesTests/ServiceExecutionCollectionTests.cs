using Xunit;
using Microsoft.Extensions.DependencyInjection;
using lms.Services;
using lms.Abstractions.Interfaces;
using lms.Services.Repository;
using lms.Abstractions.Models.States;
using lms_server.Repository;

namespace lms.Tests.ServicesTests
{
    public class ServiceExecutionCollectionTests
    {
        private readonly IServiceCollection _services;

        public ServiceExecutionCollectionTests()
        {
            _services = new ServiceCollection();
        }

        [Fact]
        public void AddServices_ShouldRegisterPendingState()
        {
            // Act
            _services.AddServices();
            var serviceProvider = _services.BuildServiceProvider();

            // Assert
            var service = serviceProvider.GetService<PendingState>();
            Assert.NotNull(service);
        }

        [Fact]
        public void AddServices_ShouldRegisterApprovedState()
        {
            // Act
            _services.AddServices();
            var serviceProvider = _services.BuildServiceProvider();

            // Assert
            var service = serviceProvider.GetService<ApprovedState>();
            Assert.NotNull(service);
        }

        [Fact]
        public void AddServices_ShouldRegisterCancelledState()
        {
            // Act
            _services.AddServices();
            var serviceProvider = _services.BuildServiceProvider();

            // Assert
            var service = serviceProvider.GetService<CancelledState>();
            Assert.NotNull(service);
        }

        [Fact]
        public void AddServices_ShouldRegisterReturnState()
        {
            // Act
            _services.AddServices();
            var serviceProvider = _services.BuildServiceProvider();

            // Assert
            var service = serviceProvider.GetService<ReturnState>();
            Assert.NotNull(service);
        }

        [Fact]
        public void AddServices_ShouldRegisterOverdueState()
        {
            // Act
            _services.AddServices();
            var serviceProvider = _services.BuildServiceProvider();

            // Assert
            var service = serviceProvider.GetService<OverdueState>();
            Assert.NotNull(service);
        }
    }
}