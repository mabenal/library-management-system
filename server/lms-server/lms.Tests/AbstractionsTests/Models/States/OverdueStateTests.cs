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
    public class OverdueStateTests
    {
        private readonly OverdueState _overdueState;
        private readonly Mock<ILmsDbContext> _dbContextMock;
        private readonly BookRequest _bookRequest;

        public OverdueStateTests()
        {
            _overdueState = new OverdueState();
            _dbContextMock = new Mock<ILmsDbContext>();
            _bookRequest = BookRequest.CreatePendingRequest(
                "Test Book Request",
                DateTime.Now,
                DateTime.Now.AddDays(7),
                new Client { Id = Guid.NewGuid() },
                new Book { Id = Guid.NewGuid() }
            );
            _bookRequest.SetState(_overdueState);
        }

        [Fact]
        public async Task Approve_ShouldThrowGlobalException()
        {
            // Act & Assert
            var exception = await Assert.ThrowsAsync<GlobalException>(() => _overdueState.Approve(_bookRequest, _dbContextMock.Object));
            Assert.Equal("Overdue book requests cannot be approved.", exception.Message);
        }

        [Fact]
        public async Task Cancel_ShouldThrowGlobalException()
        {
            // Act & Assert
            var exception = await Assert.ThrowsAsync<GlobalException>(() => _overdueState.Cancel(_bookRequest, _dbContextMock.Object));
            Assert.Equal("Overdue book requests cannot be canceled.", exception.Message);
        }

        [Fact]
        public async Task Pending_ShouldThrowGlobalException()
        {
            // Act & Assert
            var exception = await Assert.ThrowsAsync<GlobalException>(() => _overdueState.Pending(_bookRequest));
            Assert.Equal("Overdue book requests cannot be pending.", exception.Message);
        }

        [Fact]
        public async Task Return_ShouldUpdateBookRequestStatusToReturned()
        {
            // Arrange
            _overdueState.Return(_bookRequest, _dbContextMock.Object);

            // Act
            var result = _bookRequest.Status;

            // Assert
            Assert.Equal("Returned", result);
        }

        [Fact]
        public async Task MarkAsOverdue_ShouldThrowGlobalException()
        {
            // Act & Assert
            var exception = await Assert.ThrowsAsync<GlobalException>(() => _overdueState.MarkAsOverdue(_bookRequest, _dbContextMock.Object));
            Assert.Equal("The book request is already overdue.", exception.Message);
        }
    }
}