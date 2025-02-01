using System;
using System.Threading.Tasks;
using Xunit;
using Moq;
using lms.Abstractions.Models;
using lms.Abstractions.Interfaces;
using lms.Abstractions.Models.States;

namespace lms.Tests.AbstractionsTests.Models
{
    public class BookRequestTests
    {
        private readonly Mock<ILmsDbContext> _dbContextMock;
        private readonly BookRequest _bookRequest;
        private readonly Client _client;
        private readonly Book _book;

        public BookRequestTests()
        {
            _dbContextMock = new Mock<ILmsDbContext>();
            _client = new Client { Id = Guid.NewGuid(), Name = "Test Client" };
            _book = new Book { Id = Guid.NewGuid(), Title = "Test Book", NumberOfCopies = 1 };
            _bookRequest = BookRequest.CreatePendingRequest("Test Book Request", DateTime.Now, DateTime.Now.AddDays(7), _client, _book);
        }

        [Fact]
        public async Task Approve_ShouldUpdateBookRequestStatusToApproved()
        {
            // Arrange
            _bookRequest.Client = _client; 
            _bookRequest.Book = _book;

            // Act
            await _bookRequest.Approve(_dbContextMock.Object);

            // Assert
            Assert.Equal("Approved", _bookRequest.Status);
            _dbContextMock.Verify(db => db.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task Cancel_ShouldUpdateBookRequestStatusToCancelled()
        {
            // Arrange
            _bookRequest.Client = _client; 
            _bookRequest.Book = _book; 

            // Act
            await _bookRequest.Cancel(_dbContextMock.Object);

            // Assert
            Assert.Equal("Cancelled", _bookRequest.Status);
            _dbContextMock.Verify(db => db.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task Return_ShouldUpdateBookRequestStatusToReturned()
        {
            // Arrange
            _bookRequest.Client = _client; 
            _bookRequest.Book = _book; 
            await _bookRequest.Approve(_dbContextMock.Object); 

            // Act
            await _bookRequest.Return(_dbContextMock.Object);

            // Assert
            Assert.Equal("Returned", _bookRequest.Status);
            _dbContextMock.Verify(db => db.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task MarkAsOverdue_ShouldUpdateBookRequestStatusToOverdue()
        {
            // Arrange
            _bookRequest.Client = _client; 
            _bookRequest.Book = _book; 
            await _bookRequest.Approve(_dbContextMock.Object); 

            // Act
            await _bookRequest.MarkAsOverdue(_dbContextMock.Object);

            // Assert
            Assert.Equal("Overdue", _bookRequest.Status);
            _dbContextMock.Verify(db => db.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public void CreatePendingRequest_ShouldSetStatusToPending()
        {
            // Assert
            Assert.Equal("Pending", _bookRequest.Status);
        }
    }
}