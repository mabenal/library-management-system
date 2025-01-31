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
    public class ApprovedStateTests
    {
        private readonly ApprovedState _approvedState;
        private readonly Mock<ILmsDbContext> _dbContextMock;
        private readonly BookRequest _bookRequest;

        public ApprovedStateTests()
        {
            _approvedState = new ApprovedState();
            _dbContextMock = new Mock<ILmsDbContext>();
            _bookRequest = BookRequest.CreatePendingRequest(
                "Test Book Request",
                DateTime.Now,
                DateTime.Now.AddDays(7),
                new Client { Id = Guid.NewGuid() },
                new Book { Id = Guid.NewGuid() }
            );
            _bookRequest.SetState(_approvedState);
        }

        [Fact]
        public async Task Approve_ShouldThrowGlobalException()
        {
            // Act & Assert
            var exception = await Assert.ThrowsAsync<GlobalException>(() => _approvedState.Approve(_bookRequest, _dbContextMock.Object));
            Assert.Equal("The book request is already approved.", exception.Message);
        }

        [Fact]
        public async Task Cancel_ShouldThrowGlobalException()
        {
            // Act & Assert
            var exception = await Assert.ThrowsAsync<GlobalException>(() => _approvedState.Cancel(_bookRequest, _dbContextMock.Object));
            Assert.Equal("Approved book requests cannot be canceled.", exception.Message);
        }

        [Fact]
        public async Task Pending_ShouldThrowGlobalException()
        {
            // Act & Assert
            var exception = await Assert.ThrowsAsync<GlobalException>(() => _approvedState.Pending(_bookRequest));
            Assert.Equal("Approved book requests cannot be pending.", exception.Message);
        }

        [Fact]
        public async Task Return_ShouldUpdateBookRequestStatusToReturned()
        {
            // Arrange
            _approvedState.Return(_bookRequest, _dbContextMock.Object);

            // Act
            var result = _bookRequest.Status;

            // Assert
            Assert.Equal("Returned", result);
        }

        [Fact]
        public async Task MarkAsOverdue_ShouldUpdateBookRequestStatusToOverdue()
        {
            // Arrange
            _approvedState.MarkAsOverdue(_bookRequest, _dbContextMock.Object);

            // Act
            var result = _bookRequest.Status;

            // Assert
            Assert.Equal("Overdue", result);
        }
    }
}