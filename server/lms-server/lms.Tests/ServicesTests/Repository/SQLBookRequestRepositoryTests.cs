using Xunit;
using Moq;
using lms.Abstractions.Data;
using lms.Abstractions.Models;
using lms.Abstractions.Exceptions;
using lms.Services.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MockQueryable.Moq;
using lms.Abstractions.Interfaces;

namespace lms.Tests.ServicesTests.Repository
{
    public class SQLBookRequestRepositoryTests
    {
        private readonly Mock<ILmsDbContext> _dbContextMock;
        private readonly SQLBookRequestRepository _repository;

        public SQLBookRequestRepositoryTests()
        {
            _dbContextMock = new Mock<ILmsDbContext>();
            _repository = new SQLBookRequestRepository(_dbContextMock.Object);
        }

        [Fact]
        public async Task AddNewRequest_ShouldAddRequest_WhenValid()
        {
            // Arrange
            var client = new Client { Id = Guid.NewGuid(), Name = "Test Client" };
            var book = new Book { Id = Guid.NewGuid(), Title = "Test Book", NumberOfCopies = 1 };
            var bookRequest = BookRequest.CreatePendingRequest("Test Book", DateTime.Now, DateTime.Now.AddDays(15), client, book);
            var bookRequests = new List<BookRequest>().AsQueryable().BuildMockDbSet();

            _dbContextMock.Setup(db => db.BookRequests).Returns(bookRequests.Object);
            _dbContextMock.Setup(db => db.Books.FindAsync(bookRequest.BookId)).ReturnsAsync(book);
            _dbContextMock.Setup(db => db.BookRequests.AddAsync(bookRequest, default)).ReturnsAsync(new Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<BookRequest>(null));

            // Act
            var result = await _repository.AddNewRequest(bookRequest);

            // Assert
            Assert.NotNull(result);
            _dbContextMock.Verify(db => db.BookRequests.AddAsync(bookRequest, default), Times.Once);
            _dbContextMock.Verify(db => db.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task AddNewRequest_ShouldThrowException_WhenBookAlreadyRequested()
        {
            // Arrange
            var client = new Client { Id = Guid.NewGuid(), Name = "Test Client" };
            var book = new Book { Id = Guid.NewGuid(), Title = "Test Book", NumberOfCopies = 1 };
            var bookRequest = BookRequest.CreatePendingRequest("Test Book", DateTime.Now, DateTime.Now.AddDays(15), client, book);
            var existingBookRequest = BookRequest.CreatePendingRequest("Test Book", DateTime.Now, DateTime.Now.AddDays(15), client, book);
            var bookRequests = new List<BookRequest> { existingBookRequest }.AsQueryable().BuildMockDbSet();

            _dbContextMock.Setup(db => db.BookRequests).Returns(bookRequests.Object);
            _dbContextMock.Setup(db => db.BookRequests.FirstOrDefaultAsync(It.IsAny<Expression<Func<BookRequest, bool>>>(), default)).ReturnsAsync(existingBookRequest);

            // Act & Assert
            await Assert.ThrowsAsync<GlobalException>(() => _repository.AddNewRequest(bookRequest));
        }

        [Fact]
        public async Task GetAllBookRequestsAsync_ShouldReturnRequests()
        {
            // Arrange
            var client = new Client { Id = Guid.NewGuid(), Name = "Test Client" };
            var book = new Book { Id = Guid.NewGuid(), Title = "Test Book", NumberOfCopies = 1 };
            var bookRequest = BookRequest.CreatePendingRequest("Test Book", DateTime.Now, DateTime.Now.AddDays(15), client, book);
            var bookRequests = new List<BookRequest> { bookRequest }.AsQueryable().BuildMockDbSet();

            _dbContextMock.Setup(db => db.BookRequests).Returns(bookRequests.Object);

            // Act
            var result = await _repository.GetAllBookRequestsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
        }

        [Fact]
        public async Task GetBookRequestsByClientId_ShouldReturnRequests()
        {
            // Arrange
            var client = new Client { Id = Guid.NewGuid(), Name = "Test Client" };
            var book = new Book { Id = Guid.NewGuid(), Title = "Test Book", NumberOfCopies = 1 };
            var bookRequest = BookRequest.CreatePendingRequest("Test Book", DateTime.Now, DateTime.Now.AddDays(15), client, book);
            var bookRequests = new List<BookRequest> { bookRequest }.AsQueryable().BuildMockDbSet();

            _dbContextMock.Setup(db => db.BookRequests).Returns(bookRequests.Object);
            _dbContextMock.Setup(db => db.BookRequests.Where(It.IsAny<Expression<Func<BookRequest, bool>>>()).ToListAsync(It.IsAny<CancellationToken>())).ReturnsAsync(bookRequests.Object.ToList());

            // Act
            var result = await _repository.GetBookRequestsByClientId(client.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(bookRequest, result.First());
        }

        [Fact]
        public async Task ApproveRequest_ShouldApproveRequest_WhenValid()
        {
            // Arrange
            var client = new Client { Id = Guid.NewGuid(), Name = "Test Client" };
            var book = new Book { Id = Guid.NewGuid(), Title = "Test Book", NumberOfCopies = 1 };
            var bookRequest = BookRequest.CreatePendingRequest("Test Book", DateTime.Now, DateTime.Now.AddDays(15), client, book);
            bookRequest.Status = "Pending";
            var bookRequests = new List<BookRequest> { bookRequest }.AsQueryable().BuildMockDbSet();

            _dbContextMock.Setup(db => db.BookRequests).Returns(bookRequests.Object);
            _dbContextMock.Setup(db => db.BookRequests.SingleOrDefaultAsync(It.IsAny<Expression<Func<BookRequest, bool>>>(), default)).ReturnsAsync(bookRequest);

            // Act
            var result = await _repository.ApproveRequest(client.Id, book.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Approved", result.Status);
            _dbContextMock.Verify(db => db.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task ApproveRequest_ShouldThrowException_WhenRequestNotFound()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var bookId = Guid.NewGuid();
            _dbContextMock.Setup(db => db.BookRequests.SingleOrDefaultAsync(It.IsAny<Expression<Func<BookRequest, bool>>>(), default)).ReturnsAsync((BookRequest)null);

            // Act & Assert
            await Assert.ThrowsAsync<GlobalException>(() => _repository.ApproveRequest(clientId, bookId));
        }

        [Fact]
        public async Task CancelRequest_ShouldCancelRequest_WhenValid()
        {
            // Arrange
            var client = new Client { Id = Guid.NewGuid(), Name = "Test Client" };
            var book = new Book { Id = Guid.NewGuid(), Title = "Test Book", NumberOfCopies = 1 };
            var bookRequest = BookRequest.CreatePendingRequest("Test Book", DateTime.Now, DateTime.Now.AddDays(15), client, book);
            bookRequest.Status = "Pending";
            var bookRequests = new List<BookRequest> { bookRequest }.AsQueryable().BuildMockDbSet();

            _dbContextMock.Setup(db => db.BookRequests).Returns(bookRequests.Object);
            _dbContextMock.Setup(db => db.BookRequests.SingleOrDefaultAsync(It.IsAny<Expression<Func<BookRequest, bool>>>(), default)).ReturnsAsync(bookRequest);

            // Act
            var result = await _repository.CancelRequest(client.Id, book.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Cancelled", result.Status);
            _dbContextMock.Verify(db => db.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task CancelRequest_ShouldThrowException_WhenRequestNotFound()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var bookId = Guid.NewGuid();
            _dbContextMock.Setup(db => db.BookRequests.SingleOrDefaultAsync(It.IsAny<Expression<Func<BookRequest, bool>>>(), default)).ReturnsAsync((BookRequest)null);

            // Act & Assert
            await Assert.ThrowsAsync<GlobalException>(() => _repository.CancelRequest(clientId, bookId));
        }

        [Fact]
        public async Task ReturnRequest_ShouldReturnRequest_WhenValid()
        {
            // Arrange
            var client = new Client { Id = Guid.NewGuid(), Name = "Test Client" };
            var book = new Book { Id = Guid.NewGuid(), Title = "Test Book", NumberOfCopies = 1 };
            var bookRequest = BookRequest.CreatePendingRequest("Test Book", DateTime.Now, DateTime.Now.AddDays(15), client, book);
            bookRequest.Status = "Approved";
            var bookRequests = new List<BookRequest> { bookRequest }.AsQueryable().BuildMockDbSet();

            _dbContextMock.Setup(db => db.BookRequests).Returns(bookRequests.Object);
            _dbContextMock.Setup(db => db.BookRequests.SingleOrDefaultAsync(It.IsAny<Expression<Func<BookRequest, bool>>>(), default)).ReturnsAsync(bookRequest);

            // Act
            var result = await _repository.ReturnRequest(client.Id, book.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Returned", result.Status);
            _dbContextMock.Verify(db => db.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task ReturnRequest_ShouldThrowException_WhenRequestNotFound()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var bookId = Guid.NewGuid();
            var bookRequests = new List<BookRequest>().AsQueryable().BuildMockDbSet();

            _dbContextMock.Setup(db => db.BookRequests).Returns(bookRequests.Object);
            _dbContextMock.Setup(db => db.BookRequests.SingleOrDefaultAsync(It.IsAny<Expression<Func<BookRequest, bool>>>(), It.IsAny<CancellationToken>())).ReturnsAsync((BookRequest)null);

            // Act & Assert
            await Assert.ThrowsAsync<GlobalException>(() => _repository.ReturnRequest(clientId, bookId));
        }

        [Fact]
        public async Task OverdueRequest_ShouldOverdueRequest_WhenValid()
        {
            // Arrange
            var client = new Client { Id = Guid.NewGuid(), Name = "Test Client" };
            var book = new Book { Id = Guid.NewGuid(), Title = "Test Book", NumberOfCopies = 1 };
            var bookRequest = BookRequest.CreatePendingRequest("Test Book", DateTime.Now, DateTime.Now.AddDays(15), client, book);
            bookRequest.Status = "Approved";
            var bookRequests = new List<BookRequest> { bookRequest }.AsQueryable().BuildMockDbSet();

            _dbContextMock.Setup(db => db.BookRequests).Returns(bookRequests.Object);
            _dbContextMock.Setup(db => db.BookRequests.SingleOrDefaultAsync(It.IsAny<Expression<Func<BookRequest, bool>>>(), default)).ReturnsAsync(bookRequest);

            // Act
            var result = await _repository.OverdueRequest(client.Id, book.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Overdue", result.Status);
            _dbContextMock.Verify(db => db.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task OverdueRequest_ShouldThrowException_WhenRequestNotFound()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var bookId = Guid.NewGuid();
            _dbContextMock.Setup(db => db.BookRequests.SingleOrDefaultAsync(It.IsAny<Expression<Func<BookRequest, bool>>>(), default)).ReturnsAsync((BookRequest)null);

            // Act & Assert
            await Assert.ThrowsAsync<GlobalException>(() => _repository.OverdueRequest(clientId, bookId));
        }
    }
}