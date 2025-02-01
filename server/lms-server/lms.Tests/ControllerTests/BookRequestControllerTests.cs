using Xunit;
using Moq;
using AutoMapper;
using lms.Abstractions.Interfaces;
using lms.Abstractions.Models;
using lms.Abstractions.Models.DTO;
using lms.Abstractions.Exceptions;
using lms.Peer.Controllers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace lms.Tests.ControllerTests
{
    public class BookRequestControllerTests
    {
        private readonly Mock<IBookRequestRepository> _bookRequestRepositoryMock;
        private readonly Mock<IBooksRepository> _booksRepositoryMock;
        private readonly Mock<IClientRepository> _clientRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly BookRequestController _controller;

        public BookRequestControllerTests()
        {
            _bookRequestRepositoryMock = new Mock<IBookRequestRepository>();
            _booksRepositoryMock = new Mock<IBooksRepository>();
            _clientRepositoryMock = new Mock<IClientRepository>();
            _mapperMock = new Mock<IMapper>();
            _controller = new BookRequestController(_bookRequestRepositoryMock.Object, _booksRepositoryMock.Object, _clientRepositoryMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task GetAllBookRequests_ReturnsOkResult_WithListOfBookRequestDtos()
        {
            // Arrange
            var bookRequests = new List<BookRequest> { BookRequest.CreatePendingRequest("Title", DateTime.Now, DateTime.Now.AddDays(7), new Client(), new Book()) };
            var bookRequestDtos = new List<BookRequestDto> { new BookRequestDto() };

            _bookRequestRepositoryMock.Setup(repo => repo.GetAllBookRequestsAsync()).ReturnsAsync(bookRequests);
            _mapperMock.Setup(m => m.Map<List<BookRequestDto>>(bookRequests)).Returns(bookRequestDtos);

            // Act
            var result = await _controller.GetAllBookRequests();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<List<BookRequestDto>>(okResult.Value);
            Assert.Equal(bookRequestDtos, returnValue);
        }

        [Fact]
        public async Task GetAllBookRequests_ThrowsGlobalException()
        {
            // Arrange
            _bookRequestRepositoryMock.Setup(repo => repo.GetAllBookRequestsAsync()).ThrowsAsync(new Exception("Test exception"));

            // Act & Assert
            await Assert.ThrowsAsync<GlobalException>(() => _controller.GetAllBookRequests());
        }

        [Fact]
        public async Task AddNewRequest_ReturnsOkResult_WithBookRequestDto()
        {
            // Arrange
            var bookRequestDto = new BookRequestDto();
            var bookRequest = BookRequest.CreatePendingRequest("Title", DateTime.Now, DateTime.Now.AddDays(7), new Client(), new Book());

            _mapperMock.Setup(m => m.Map<BookRequest>(bookRequestDto)).Returns(bookRequest);
            _bookRequestRepositoryMock.Setup(repo => repo.AddNewRequest(bookRequest)).ReturnsAsync(bookRequest);
            _mapperMock.Setup(m => m.Map<BookRequestDto>(bookRequest)).Returns(bookRequestDto);

            // Act
            var result = await _controller.AddNewRequest(bookRequestDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<BookRequestDto>(okResult.Value);
            Assert.Equal(bookRequestDto, returnValue);
        }

        [Fact]
        public async Task AddNewRequest_ReturnsStatusCode403_WhenGlobalExceptionThrown()
        {
            // Arrange
            var bookRequestDto = new BookRequestDto();
            _bookRequestRepositoryMock.Setup(repo => repo.AddNewRequest(It.IsAny<BookRequest>())).ThrowsAsync(new GlobalException("Test exception"));

            // Act
            var result = await _controller.AddNewRequest(bookRequestDto);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(403, statusCodeResult.StatusCode);
            Assert.Equal("Test exception", statusCodeResult.Value);
        }

        [Fact]
        public async Task AddNewRequest_ThrowsGlobalException()
        {
            // Arrange
            var bookRequestDto = new BookRequestDto();
            _bookRequestRepositoryMock.Setup(repo => repo.AddNewRequest(It.IsAny<BookRequest>())).ThrowsAsync(new Exception("Test exception"));

            // Act & Assert
            await Assert.ThrowsAsync<GlobalException>(() => _controller.AddNewRequest(bookRequestDto));
        }

        [Fact]
        public async Task GetBookRequestsByClientId_ReturnsOkResult_WithListOfBookRequestDtos()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var bookRequests = new List<BookRequest> { BookRequest.CreatePendingRequest("Title", DateTime.Now, DateTime.Now.AddDays(7), new Client(), new Book()) };
            var bookRequestDtos = new List<BookRequestDto> { new BookRequestDto() };

            _bookRequestRepositoryMock.Setup(repo => repo.GetBookRequestsByClientId(clientId)).ReturnsAsync(bookRequests);
            _mapperMock.Setup(m => m.Map<List<BookRequestDto>>(bookRequests)).Returns(bookRequestDtos);

            // Act
            var result = await _controller.GetBookRequestsByClientId(clientId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<List<BookRequestDto>>(okResult.Value);
            Assert.Equal(bookRequestDtos, returnValue);
        }

        [Fact]
        public async Task GetBookRequestsByClientId_ThrowsGlobalException()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            _bookRequestRepositoryMock.Setup(repo => repo.GetBookRequestsByClientId(clientId)).ThrowsAsync(new Exception("Test exception"));

            // Act & Assert
            await Assert.ThrowsAsync<GlobalException>(() => _controller.GetBookRequestsByClientId(clientId));
        }

        [Fact]
        public async Task CancelRequest_ReturnsOkResult_WithBookRequestDto()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var bookId = Guid.NewGuid();
            var bookRequest = BookRequest.CreatePendingRequest("Title", DateTime.Now, DateTime.Now.AddDays(7), new Client(), new Book());
            var bookRequestDto = new BookRequestDto();

            _bookRequestRepositoryMock.Setup(repo => repo.CancelRequest(clientId, bookId)).ReturnsAsync(bookRequest);
            _mapperMock.Setup(m => m.Map<BookRequestDto>(bookRequest)).Returns(bookRequestDto);

            // Act
            var result = await _controller.CancelRequest(clientId, bookId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<BookRequestDto>(okResult.Value);
            Assert.Equal(bookRequestDto, returnValue);
        }

        [Fact]
        public async Task CancelRequest_ReturnsStatusCode403_WhenGlobalExceptionThrown()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var bookId = Guid.NewGuid();
            _bookRequestRepositoryMock.Setup(repo => repo.CancelRequest(clientId, bookId)).ThrowsAsync(new GlobalException("Test exception"));

            // Act
            var result = await _controller.CancelRequest(clientId, bookId);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(403, statusCodeResult.StatusCode);
            Assert.Equal("Test exception", statusCodeResult.Value);
        }

        [Fact]
        public async Task CancelRequest_ThrowsGlobalException()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var bookId = Guid.NewGuid();
            _bookRequestRepositoryMock.Setup(repo => repo.CancelRequest(clientId, bookId)).ThrowsAsync(new Exception("Test exception"));

            // Act & Assert
            await Assert.ThrowsAsync<GlobalException>(() => _controller.CancelRequest(clientId, bookId));
        }

        [Fact]
        public async Task ApproveRequest_ReturnsOkResult_WithBookRequestDto()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var bookId = Guid.NewGuid();
            var bookRequest = BookRequest.CreatePendingRequest("Title", DateTime.Now, DateTime.Now.AddDays(7), new Client(), new Book());
            var bookRequestDto = new BookRequestDto();

            _bookRequestRepositoryMock.Setup(repo => repo.ApproveRequest(clientId, bookId)).ReturnsAsync(bookRequest);
            _mapperMock.Setup(m => m.Map<BookRequestDto>(bookRequest)).Returns(bookRequestDto);

            // Act
            var result = await _controller.ApproveRequest(clientId, bookId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<BookRequestDto>(okResult.Value);
            Assert.Equal(bookRequestDto, returnValue);
        }

        [Fact]
        public async Task ApproveRequest_ReturnsStatusCode403_WhenGlobalExceptionThrown()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var bookId = Guid.NewGuid();
            _bookRequestRepositoryMock.Setup(repo => repo.ApproveRequest(clientId, bookId)).ThrowsAsync(new GlobalException("Test exception"));

            // Act
            var result = await _controller.ApproveRequest(clientId, bookId);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(403, statusCodeResult.StatusCode);
            Assert.Equal("Test exception", statusCodeResult.Value);
        }

        [Fact]
        public async Task ApproveRequest_ThrowsGlobalException()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var bookId = Guid.NewGuid();
            _bookRequestRepositoryMock.Setup(repo => repo.ApproveRequest(clientId, bookId)).ThrowsAsync(new Exception("Test exception"));

            // Act & Assert
            await Assert.ThrowsAsync<GlobalException>(() => _controller.ApproveRequest(clientId, bookId));
        }

        [Fact]
        public async Task ReturnRequest_ReturnsOkResult_WithBookRequestDto()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var bookId = Guid.NewGuid();
            var bookRequest = BookRequest.CreatePendingRequest("Title", DateTime.Now, DateTime.Now.AddDays(7), new Client(), new Book());
            var bookRequestDto = new BookRequestDto();

            _bookRequestRepositoryMock.Setup(repo => repo.ReturnRequest(clientId, bookId)).ReturnsAsync(bookRequest);
            _mapperMock.Setup(m => m.Map<BookRequestDto>(bookRequest)).Returns(bookRequestDto);

            // Act
            var result = await _controller.ReturnRequest(clientId, bookId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<BookRequestDto>(okResult.Value);
            Assert.Equal(bookRequestDto, returnValue);
        }

        [Fact]
        public async Task ReturnRequest_ReturnsStatusCode403_WhenGlobalExceptionThrown()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var bookId = Guid.NewGuid();
            _bookRequestRepositoryMock.Setup(repo => repo.ReturnRequest(clientId, bookId)).ThrowsAsync(new GlobalException("Test exception"));

            // Act
            var result = await _controller.ReturnRequest(clientId, bookId);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(403, statusCodeResult.StatusCode);
            Assert.Equal("Test exception", statusCodeResult.Value);
        }

        [Fact]
        public async Task ReturnRequest_ThrowsGlobalException()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var bookId = Guid.NewGuid();
            _bookRequestRepositoryMock.Setup(repo => repo.ReturnRequest(clientId, bookId)).ThrowsAsync(new Exception("Test exception"));

            // Act & Assert
            await Assert.ThrowsAsync<GlobalException>(() => _controller.ReturnRequest(clientId, bookId));
        }

        [Fact]
        public async Task OverdueRequest_ReturnsOkResult_WithBookRequestDto()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var bookId = Guid.NewGuid();
            var bookRequest = BookRequest.CreatePendingRequest("Title", DateTime.Now, DateTime.Now.AddDays(7), new Client(), new Book());
            var bookRequestDto = new BookRequestDto();

            _bookRequestRepositoryMock.Setup(repo => repo.OverdueRequest(clientId, bookId)).ReturnsAsync(bookRequest);
            _mapperMock.Setup(m => m.Map<BookRequestDto>(bookRequest)).Returns(bookRequestDto);

            // Act
            var result = await _controller.OverdueRequest(clientId, bookId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<BookRequestDto>(okResult.Value);
            Assert.Equal(bookRequestDto, returnValue);
        }

        [Fact]
        public async Task OverdueRequest_ReturnsStatusCode403_WhenGlobalExceptionThrown()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var bookId = Guid.NewGuid();
            _bookRequestRepositoryMock.Setup(repo => repo.OverdueRequest(clientId, bookId)).ThrowsAsync(new GlobalException("Test exception"));

            // Act
            var result = await _controller.OverdueRequest(clientId, bookId);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(403, statusCodeResult.StatusCode);
            Assert.Equal("Test exception", statusCodeResult.Value);
        }

        [Fact]
        public async Task OverdueRequest_ThrowsGlobalException()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var bookId = Guid.NewGuid();
            _bookRequestRepositoryMock.Setup(repo => repo.OverdueRequest(clientId, bookId)).ThrowsAsync(new Exception("Test exception"));

            // Act & Assert
            await Assert.ThrowsAsync<GlobalException>(() => _controller.OverdueRequest(clientId, bookId));
        }
    }
}