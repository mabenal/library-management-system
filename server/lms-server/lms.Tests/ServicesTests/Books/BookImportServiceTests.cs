using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using lms.Abstractions.Data;
using lms.Abstractions.Interfaces;
using lms.Abstractions.Models;
using lms.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace lms.Tests.ServicesTests.Books
{
    public class BookImportServiceTests
    {
        private readonly Mock<ILmsDbContext> _dbContextMock;
        private readonly BookImportService _bookImportService;

        public BookImportServiceTests()
        {
            _dbContextMock = new Mock<ILmsDbContext>();
            _bookImportService = new BookImportService(_dbContextMock.Object);
        }

        [Fact]
        public async Task ImportBooksAsync_AddsBooks_WhenDatabaseIsEmpty()
        {
            // Arrange
            var books = new List<Book> { new Book { Id = Guid.NewGuid(), Title = "Test Book" } };
            var jsonData = JsonConvert.SerializeObject(books);
            var filePath = "test.json";

            await File.WriteAllTextAsync(filePath, jsonData);

            var mockBookSet = new Mock<DbSet<Book>>();
            var emptyBooks = new List<Book>().AsQueryable();

            mockBookSet.As<IQueryable<Book>>().Setup(m => m.Provider).Returns(emptyBooks.Provider);
            mockBookSet.As<IQueryable<Book>>().Setup(m => m.Expression).Returns(emptyBooks.Expression);
            mockBookSet.As<IQueryable<Book>>().Setup(m => m.ElementType).Returns(emptyBooks.ElementType);
            mockBookSet.As<IQueryable<Book>>().Setup(m => m.GetEnumerator()).Returns(emptyBooks.GetEnumerator());

            _dbContextMock.Setup(db => db.Books).Returns(mockBookSet.Object);
            _dbContextMock.Setup(db => db.SaveChangesAsync(default)).ReturnsAsync(1);

            // Act
            await _bookImportService.ImportBooksAsync(filePath);

            // Assert
            mockBookSet.Verify(m => m.Add(It.IsAny<Book>()), Times.Once);
            _dbContextMock.Verify(db => db.SaveChangesAsync(default), Times.Once);

            // Cleanup
            File.Delete(filePath);
        }

        [Fact]
        public async Task ImportBooksAsync_DoesNotAddBooks_WhenDatabaseIsNotEmpty()
        {
            // Arrange
            var books = new List<Book> { new Book { Id = Guid.NewGuid(), Title = "Test Book" } };
            var jsonData = JsonConvert.SerializeObject(books);
            var filePath = "test.json";

            await File.WriteAllTextAsync(filePath, jsonData);

            var mockBookSet = new Mock<DbSet<Book>>();
            var existingBooks = new List<Book> { new Book { Id = Guid.NewGuid(), Title = "Existing Book" } }.AsQueryable();

            mockBookSet.As<IQueryable<Book>>().Setup(m => m.Provider).Returns(existingBooks.Provider);
            mockBookSet.As<IQueryable<Book>>().Setup(m => m.Expression).Returns(existingBooks.Expression);
            mockBookSet.As<IQueryable<Book>>().Setup(m => m.ElementType).Returns(existingBooks.ElementType);
            mockBookSet.As<IQueryable<Book>>().Setup(m => m.GetEnumerator()).Returns(existingBooks.GetEnumerator());

            _dbContextMock.Setup(db => db.Books).Returns(mockBookSet.Object);
            _dbContextMock.Setup(db => db.SaveChangesAsync(default)).ReturnsAsync(1);

            // Act
            await _bookImportService.ImportBooksAsync(filePath);

            // Assert
            mockBookSet.Verify(m => m.Add(It.IsAny<Book>()), Times.Never);
            _dbContextMock.Verify(db => db.SaveChangesAsync(default), Times.Never);
        }

        [Fact]
        public async Task ImportBooksAsync_ThrowsException_WhenFileNotFound()
        {
            // Arrange
            var filePath = "nonexistent.json";

            // Act & Assert
            await Assert.ThrowsAsync<FileNotFoundException>(() => _bookImportService.ImportBooksAsync(filePath));
        }

        [Fact]
        public async Task ImportBooksAsync_ThrowsException_WhenJsonIsInvalid()
        {
            // Arrange
            var invalidJson = "invalid json";
            var filePath = "test.json";

            await File.WriteAllTextAsync(filePath, invalidJson);

            // Act & Assert
            await Assert.ThrowsAsync<JsonReaderException>(() => _bookImportService.ImportBooksAsync(filePath));

            // Cleanup
            File.Delete(filePath);
        }
    }
}