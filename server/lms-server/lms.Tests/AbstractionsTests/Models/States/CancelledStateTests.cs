using Xunit;
using Moq;
using lms.Abstractions.Data;
using lms.Abstractions.Models;
using lms.Abstractions.Models.States;
using lms.Abstractions.Exceptions;
using System;
using System.Threading.Tasks;
using lms.Abstractions.Interfaces;

namespace lms.Tests.AbstractionsTests.Models.States
{
    public class CancelledStateTests
    {
        private readonly CancelledState _cancelledState;
        private readonly Mock<ILmsDbContext> _dbContextMock;
        private readonly BookRequest _bookRequest;

        public CancelledStateTests()
        {
            _cancelledState = new CancelledState();
            _dbContextMock = new Mock<ILmsDbContext>();
            _bookRequest = BookRequest.CreatePendingRequest(
                "Test Book Request",
                DateTime.Now,
                DateTime.Now.AddDays(7),
                new Client { Id = Guid.NewGuid() },
                new Book { Id = Guid.NewGuid() }
            );
            _bookRequest.SetState(_cancelledState);
        }

        [Fact]
        public async Task Approve_ShouldThrowGlobalException()
        {
            // Act & Assert
            var exception = await Assert.ThrowsAsync<GlobalException>(() => _cancelledState.Approve(_bookRequest, _dbContextMock.Object));
            Assert.Equal("Cancelled book requests cannot be approved.", exception.Message);
        }

        [Fact]
        public async Task Cancel_ShouldThrowGlobalException()
        {
            // Act & Assert
            var exception = await Assert.ThrowsAsync<GlobalException>(() => _cancelledState.Cancel(_bookRequest, _dbContextMock.Object));
            Assert.Equal("The book request is already canceled.", exception.Message);
        }

        [Fact]
        public async Task Pending_ShouldThrowGlobalException()
        {
            // Act & Assert
            var exception = await Assert.ThrowsAsync<GlobalException>(() => _cancelledState.Pending(_bookRequest));
            Assert.Equal("Cancelled book requests cannot be pending.", exception.Message);
        }

        [Fact]
        public async Task Return_ShouldThrowGlobalException()
        {
            // Act & Assert
            var exception = await Assert.ThrowsAsync<GlobalException>(() => _cancelledState.Return(_bookRequest, _dbContextMock.Object));
            Assert.Equal("Cancelled book requests cannot be returned.", exception.Message);
        }

        [Fact]
        public async Task MarkAsOverdue_ShouldThrowGlobalException()
        {
            // Act & Assert
            var exception = await Assert.ThrowsAsync<GlobalException>(() => _cancelledState.MarkAsOverdue(_bookRequest, _dbContextMock.Object));
            Assert.Equal("Cancelled book requests cannot be marked as overdue.", exception.Message);
        }
    }
}