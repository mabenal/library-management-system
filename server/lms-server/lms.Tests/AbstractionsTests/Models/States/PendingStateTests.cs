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
    public class PendingStateTests
    {
        private readonly PendingState _pendingState;
        private readonly Mock<ILmsDbContext> _dbContextMock;
        private readonly BookRequest _bookRequest;

        public PendingStateTests()
        {
            _pendingState = new PendingState();
            _dbContextMock = new Mock<ILmsDbContext>();
            _bookRequest = BookRequest.CreatePendingRequest(
                "Test Book Request",
                DateTime.Now,
                DateTime.Now.AddDays(7),
                new Client { Id = Guid.NewGuid() },
                new Book { Id = Guid.NewGuid() }
            );
            _bookRequest.SetState(_pendingState);
        }

        [Fact]
        public async Task Approve_ShouldUpdateBookRequestStatusToApproved()
        {
            // Arrange
            await _pendingState.Approve(_bookRequest, _dbContextMock.Object);

            // Act
            var result = _bookRequest.Status;

            // Assert
            Assert.Equal("Approved", result);
        }

        [Fact]
        public async Task Cancel_ShouldUpdateBookRequestStatusToCancelled()
        {
            // Arrange
            await _pendingState.Cancel(_bookRequest, _dbContextMock.Object);

            // Act
            var result = _bookRequest.Status;

            // Assert
            Assert.Equal("Cancelled", result);
        }

        [Fact]
        public async Task Pending_ShouldThrowGlobalException()
        {
            // Act & Assert
            var exception = await Assert.ThrowsAsync<GlobalException>(() => _pendingState.Pending(_bookRequest));
            Assert.Equal("The book request is already pending.", exception.Message);
        }

        [Fact]
        public async Task Return_ShouldThrowGlobalException()
        {
            // Act & Assert
            var exception = await Assert.ThrowsAsync<GlobalException>(() => _pendingState.Return(_bookRequest, _dbContextMock.Object));
            Assert.Equal("Pending book requests cannot be returned.", exception.Message);
        }

        [Fact]
        public async Task MarkAsOverdue_ShouldThrowGlobalException()
        {
            // Act & Assert
            var exception = await Assert.ThrowsAsync<GlobalException>(() => _pendingState.MarkAsOverdue(_bookRequest, _dbContextMock.Object));
            Assert.Equal("Pending book requests cannot be marked as overdue.", exception.Message);
        }
    }
}