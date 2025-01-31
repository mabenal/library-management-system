using Xunit;
using Moq;
using AutoMapper;
using lms.Abstractions.Interfaces;
using lms.Abstractions.Models;
using lms.Abstractions.Models.DTO;
using lms.Peer.Controllers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using lms.Abstractions.Exceptions;

namespace lms.Tests.ControllerTests
{
    public class BooksControllerTests
    {
        private readonly Mock<IBooksRepository> _booksRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly BooksController _controller;

        public BooksControllerTests()
        {
            _booksRepositoryMock = new Mock<IBooksRepository>();
            _mapperMock = new Mock<IMapper>();
            _controller = new BooksController(_booksRepositoryMock.Object, _mapperMock.Object, new HttpClient());
        }

        [Fact]
        public async Task SearchBooks_ReturnsOkResult_WithListOfBookDtos()
        {
            // Arrange
            var query = "test";
            var books = new List<Book> { new Book() };
            var bookDtos = new List<BookDto> { new BookDto() };

            _booksRepositoryMock.Setup(repo => repo.SearchBooksAsync(query)).ReturnsAsync(books);
            _mapperMock.Setup(m => m.Map<List<BookDto>>(books)).Returns(bookDtos);

            // Act
            var result = await _controller.SearchBooks(query);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<List<BookDto>>(okResult.Value);
            Assert.Equal(bookDtos, returnValue);
        }

        [Fact]
        public async Task SearchBooks_ThrowsGlobalException()
        {
            // Arrange
            var query = "test";
            _booksRepositoryMock.Setup(repo => repo.SearchBooksAsync(query)).ThrowsAsync(new GlobalException("Test exception"));

            // Act & Assert
            await Assert.ThrowsAsync<GlobalException>(() => _controller.SearchBooks(query));
        }

        [Fact]
        public async Task GetBookById_ReturnsOkResult_WithBookDto()
        {
            // Arrange
            var bookId = Guid.NewGuid();
            var book = new Book();
            var bookDto = new BookDto();

            _booksRepositoryMock.Setup(repo => repo.GetBookById(bookId)).ReturnsAsync(book);
            _mapperMock.Setup(m => m.Map<BookDto>(book)).Returns(bookDto);

            // Act
            var result = await _controller.GetBookById(bookId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<BookDto>(okResult.Value);
            Assert.Equal(bookDto, returnValue);
        }

        [Fact]
        public async Task GetBookById_ThrowsGlobalException()
        {
            // Arrange
            var bookId = Guid.NewGuid();
            _booksRepositoryMock.Setup(repo => repo.GetBookById(bookId)).ThrowsAsync(new GlobalException("Test exception"));

            // Act & Assert
            await Assert.ThrowsAsync<GlobalException>(() => _controller.GetBookById(bookId));
        }

        [Fact]
        public async Task GetAllBooks_ReturnsOkResult_WithListOfBookDtos()
        {
            // Arrange
            var books = new List<Book> { new Book() };
            var bookDtos = new List<BookDto> { new BookDto() };

            _booksRepositoryMock.Setup(repo => repo.GetAllBooksAsync()).ReturnsAsync(books);
            _mapperMock.Setup(m => m.Map<List<BookDto>>(books)).Returns(bookDtos);

            // Act
            var result = await _controller.GetAllBooks();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<List<BookDto>>(okResult.Value);
            Assert.Equal(bookDtos, returnValue);
        }

        [Fact]
        public async Task GetAllBooks_ThrowsGlobalException()
        {
            // Arrange
            _booksRepositoryMock.Setup(repo => repo.GetAllBooksAsync()).ThrowsAsync(new GlobalException("Test exception"));

            // Act & Assert
            await Assert.ThrowsAsync<GlobalException>(() => _controller.GetAllBooks());
        }

        [Fact]
        public async Task AddBook_ReturnsCreatedAtActionResult_WithBookDto()
        {
            // Arrange
            var bookDto = new BookDto();
            var book = new Book();

            _mapperMock.Setup(m => m.Map<Book>(bookDto)).Returns(book);
            _booksRepositoryMock.Setup(repo => repo.AddNewBook(book)).ReturnsAsync(book);
            _mapperMock.Setup(m => m.Map<BookDto>(book)).Returns(bookDto);

            // Act
            var result = await _controller.AddBook(bookDto);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnValue = Assert.IsType<BookDto>(createdAtActionResult.Value);
            Assert.Equal(bookDto, returnValue);
        }

        [Fact]
        public async Task AddBook_ReturnsStatusCode403_WhenGlobalExceptionThrown()
        {
            // Arrange
            var bookDto = new BookDto();
            _mapperMock.Setup(m => m.Map<Book>(bookDto)).Throws(new GlobalException("Test exception"));

            // Act
            var result = await _controller.AddBook(bookDto);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(403, statusCodeResult.StatusCode);
            Assert.Equal("Test exception", statusCodeResult.Value);
        }

        [Fact]
        public async Task UpdateBook_ReturnsOkResult_WithBookDto()
        {
            // Arrange
            var bookId = Guid.NewGuid();
            var bookDto = new BookDto();
            var book = new Book();

            _mapperMock.Setup(m => m.Map<Book>(bookDto)).Returns(book);
            _booksRepositoryMock.Setup(repo => repo.UpdateNewBook(bookId, book)).ReturnsAsync(book);
            _mapperMock.Setup(m => m.Map<BookDto>(book)).Returns(bookDto);

            // Act
            var result = await _controller.UpdateBook(bookId, bookDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<BookDto>(okResult.Value);
            Assert.Equal(bookDto, returnValue);
        }

        [Fact]
        public async Task UpdateBook_ReturnsNotFound_WhenBookIsNull()
        {
            // Arrange
            var bookId = Guid.NewGuid();
            var bookDto = new BookDto();

            _mapperMock.Setup(m => m.Map<Book>(bookDto)).Returns(new Book());
            _booksRepositoryMock.Setup(repo => repo.UpdateNewBook(bookId, It.IsAny<Book>())).ReturnsAsync((Book)null);

            // Act
            var result = await _controller.UpdateBook(bookId, bookDto);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task UpdateBook_ThrowsGlobalException()
        {
            // Arrange
            var bookId = Guid.NewGuid();
            var bookDto = new BookDto();
            _mapperMock.Setup(m => m.Map<Book>(bookDto)).Throws(new GlobalException("Test exception"));

            // Act & Assert
            await Assert.ThrowsAsync<GlobalException>(() => _controller.UpdateBook(bookId, bookDto));
        }

        [Fact]
        public async Task DeleteBook_ReturnsOkResult_WithBookDto()
        {
            // Arrange
            var bookId = Guid.NewGuid();
            var book = new Book();
            var bookDto = new BookDto();

            _booksRepositoryMock.Setup(repo => repo.DeleteBookAsync(bookId)).ReturnsAsync(book);
            _mapperMock.Setup(m => m.Map<BookDto>(book)).Returns(bookDto);

            // Act
            var result = await _controller.DeleteBook(bookId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<BookDto>(okResult.Value);
            Assert.Equal(bookDto, returnValue);
        }

        [Fact]
        public async Task DeleteBook_ReturnsNotFound_WhenBookIsNull()
        {
            // Arrange
            var bookId = Guid.NewGuid();
            _booksRepositoryMock.Setup(repo => repo.DeleteBookAsync(bookId)).ReturnsAsync((Book)null);

            // Act
            var result = await _controller.DeleteBook(bookId);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task DeleteBook_ThrowsGlobalException()
        {
            // Arrange
            var bookId = Guid.NewGuid();
            _booksRepositoryMock.Setup(repo => repo.DeleteBookAsync(bookId)).ThrowsAsync(new GlobalException("Test exception"));

            // Act & Assert
            await Assert.ThrowsAsync<GlobalException>(() => _controller.DeleteBook(bookId));
        }
    }
}