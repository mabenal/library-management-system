using Xunit;
using Moq;
using lms.Abstractions.Data;
using lms.Abstractions.Models;
using lms.Abstractions.Exceptions;
using lms.Services.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Threading;
using lms.Abstractions.Interfaces;
using Microsoft.EntityFrameworkCore.Query;

namespace lms.Tests.ServicesTests.Repository
{
    public class SQLBooksRepositoryTests
    {
        private readonly Mock<ILmsDbContext> _dbContextMock;
        private readonly Mock<ILogger<SQLBooksRepository>> _loggerMock;
        private readonly SQLBooksRepository _repository;

        public SQLBooksRepositoryTests()
        {
            _dbContextMock = new Mock<ILmsDbContext>();
            _loggerMock = new Mock<ILogger<SQLBooksRepository>>();
            _repository = new SQLBooksRepository(_dbContextMock.Object, _loggerMock.Object);
        }

        private static Mock<DbSet<T>> CreateDbSetMock<T>(IQueryable<T> elements) where T : class
        {
            var dbSetMock = new Mock<DbSet<T>>();
            dbSetMock.As<IQueryable<T>>().Setup(m => m.Provider).Returns(elements.Provider);
            dbSetMock.As<IQueryable<T>>().Setup(m => m.Expression).Returns(elements.Expression);
            dbSetMock.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(elements.ElementType);
            dbSetMock.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(elements.GetEnumerator());
            dbSetMock.As<IAsyncEnumerable<T>>().Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>())).Returns(new TestAsyncEnumerator<T>(elements.GetEnumerator()));
            dbSetMock.As<IQueryable<T>>().Setup(m => m.Provider).Returns(new TestAsyncQueryProvider<T>(elements.Provider));
            return dbSetMock;
        }

        [Fact]
        public async Task GetAllBooksAsync_ShouldReturnBooks()
        {
            // Arrange
            var books = new List<Book> { new Book { Id = Guid.NewGuid(), Title = "Test Book" } }.AsQueryable();
            var dbSetMock = CreateDbSetMock(books);

            _dbContextMock.Setup(db => db.Books).Returns(dbSetMock.Object);

            // Act
            var result = await _repository.GetAllBooksAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
        }

        [Fact]
        public async Task GetBookById_ShouldReturnBook_WhenBookExists()
        {
            // Arrange
            var bookId = Guid.NewGuid();
            var book = new Book { Id = bookId, Title = "Test Book" };

            _dbContextMock.Setup(db => db.Books.FindAsync(bookId)).ReturnsAsync(book);

            // Act
            var result = await _repository.GetBookById(bookId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(bookId, result.Id);
        }

        [Fact]
        public async Task GetBookById_ShouldReturnNull_WhenBookDoesNotExist()
        {
            // Arrange
            var bookId = Guid.NewGuid();

            _dbContextMock.Setup(db => db.Books.FindAsync(bookId)).ReturnsAsync((Book)null);

            // Act
            var result = await _repository.GetBookById(bookId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task AddNewBook_ShouldAddBook_WhenBookIsValid()
        {
            // Arrange
            var book = new Book { Id = Guid.NewGuid(), Title = "Test Book", ISBN = "1234567890" };
            var books = new List<Book>().AsQueryable();
            var dbSetMock = CreateDbSetMock(books);

            _dbContextMock.Setup(db => db.Books).Returns(dbSetMock.Object);
            _dbContextMock.Setup(db => db.Books.AddAsync(book, It.IsAny<CancellationToken>())).ReturnsAsync(new Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<Book>(null));

            // Act
            var result = await _repository.AddNewBook(book);

            // Assert
            Assert.NotNull(result);
            _dbContextMock.Verify(db => db.Books.AddAsync(book, It.IsAny<CancellationToken>()), Times.Once);
            _dbContextMock.Verify(db => db.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task AddNewBook_ShouldThrowException_WhenBookWithSameISBNExists()
        {
            // Arrange
            var book = new Book { Id = Guid.NewGuid(), Title = "Test Book", ISBN = "1234567890" };
            var existingBook = new Book { Id = Guid.NewGuid(), Title = "Existing Book", ISBN = "1234567890" };
            var books = new List<Book> { existingBook }.AsQueryable();
            var dbSetMock = CreateDbSetMock(books);

            _dbContextMock.Setup(db => db.Books).Returns(dbSetMock.Object);
            _dbContextMock.Setup(db => db.Books.FirstOrDefaultAsync(It.IsAny<Expression<Func<Book, bool>>>(), It.IsAny<CancellationToken>())).ReturnsAsync(existingBook);

            // Act & Assert
            await Assert.ThrowsAsync<GlobalException>(() => _repository.AddNewBook(book));
        }

        [Fact]
        public async Task SearchBooksAsync_ShouldReturnBooks_WhenQueryMatches()
        {
            // Arrange
            var books = new List<Book> { new Book { Id = Guid.NewGuid(), Title = "Test Book", Author = "Author" } }.AsQueryable();
            var dbSetMock = CreateDbSetMock(books);

            _dbContextMock.Setup(db => db.Books).Returns(dbSetMock.Object);

            // Act
            var result = await _repository.SearchBooksAsync("Test");

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
        }

        [Fact]
        public async Task SearchBooksAsync_ShouldLogError_WhenExceptionOccurs()
        {
            // Arrange
            var query = "Test";
            _dbContextMock.Setup(db => db.Books).Throws(new Exception("Test exception"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _repository.SearchBooksAsync(query));
            _loggerMock.Verify(logger => logger.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()), Times.Once);
        }

        [Fact]
        public async Task UpdateNewBook_ShouldUpdateBook_WhenBookExists()
        {
            // Arrange
            var bookId = Guid.NewGuid();
            var book = new Book { Id = bookId, Title = "Updated Book" };
            var existingBook = new Book { Id = bookId, Title = "Existing Book" };

            _dbContextMock.Setup(db => db.Books.FindAsync(bookId)).ReturnsAsync(existingBook);

            // Act
            var result = await _repository.UpdateNewBook(bookId, book);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Updated Book", result.Title);
            _dbContextMock.Verify(db => db.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task UpdateNewBook_ShouldReturnNull_WhenBookDoesNotExist()
        {
            // Arrange
            var bookId = Guid.NewGuid();
            var book = new Book { Id = bookId, Title = "Updated Book" };

            _dbContextMock.Setup(db => db.Books.FindAsync(bookId)).ReturnsAsync((Book)null);

            // Act
            var result = await _repository.UpdateNewBook(bookId, book);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteBookAsync_ShouldDeleteBook_WhenBookExists()
        {
            // Arrange
            var bookId = Guid.NewGuid();
            var book = new Book { Id = bookId, Title = "Test Book" };

            _dbContextMock.Setup(db => db.Books.FindAsync(bookId)).ReturnsAsync(book);

            // Act
            var result = await _repository.DeleteBookAsync(bookId);

            // Assert
            Assert.NotNull(result);
            _dbContextMock.Verify(db => db.Books.Remove(book), Times.Once);
            _dbContextMock.Verify(db => db.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task DeleteBookAsync_ShouldReturnNull_WhenBookDoesNotExist()
        {
            // Arrange
            var bookId = Guid.NewGuid();

            _dbContextMock.Setup(db => db.Books.FindAsync(bookId)).ReturnsAsync((Book)null);

            // Act
            var result = await _repository.DeleteBookAsync(bookId);

            // Assert
            Assert.Null(result);
        }

        private class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
        {
            private readonly IEnumerator<T> _inner;

            public TestAsyncEnumerator(IEnumerator<T> inner)
            {
                _inner = inner;
            }

            public ValueTask DisposeAsync()
            {
                _inner.Dispose();
                return ValueTask.CompletedTask;
            }

            public ValueTask<bool> MoveNextAsync()
            {
                return new ValueTask<bool>(_inner.MoveNext());
            }

            public T Current => _inner.Current;
        }

        private class TestAsyncQueryProvider<T> : IAsyncQueryProvider
        {
            private readonly IQueryProvider _inner;

            public TestAsyncQueryProvider(IQueryProvider inner)
            {
                _inner = inner;
            }

            public IQueryable CreateQuery(Expression expression)
            {
                return new TestAsyncEnumerable<T>(expression);
            }

            public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
            {
                return new TestAsyncEnumerable<TElement>(expression);
            }

            public object Execute(Expression expression)
            {
                return _inner.Execute(expression);
            }

            public TResult Execute<TResult>(Expression expression)
            {
                return _inner.Execute<TResult>(expression);
            }

            public IAsyncEnumerable<TResult> ExecuteAsync<TResult>(Expression expression)
            {
                return new TestAsyncEnumerable<TResult>(expression);
            }

            public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
            {
                return Execute<TResult>(expression);
            }
        }

        private class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
        {
            public TestAsyncEnumerable(IEnumerable<T> enumerable) : base(enumerable)
            {
            }

            public TestAsyncEnumerable(Expression expression) : base(expression)
            {
            }

            public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
            {
                return new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
            }

            IQueryProvider IQueryable.Provider => new TestAsyncQueryProvider<T>(this);
        }
    }
}