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
using lms.Services;
using Microsoft.AspNetCore.Identity;

namespace lms.Tests.ControllerTests
{
    public class BookRequestControllerTests
    {
        private readonly Mock<IBookRequestRepository> _bookRequestRepositoryMock;
        private readonly Mock<IBooksRepository> _booksRepositoryMock;
        private readonly Mock<IClientRepository> _clientRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IUserService> _mockUserService;
        private readonly BookRequestController _controller;

        public BookRequestControllerTests()
        {
            _bookRequestRepositoryMock = new Mock<IBookRequestRepository>();
            _booksRepositoryMock = new Mock<IBooksRepository>();
            _clientRepositoryMock = new Mock<IClientRepository>();
            _mapperMock = new Mock<IMapper>();
            _mockUserService = new Mock<IUserService>();
            var userManagerMock = new Mock<UserManager<ApplicationUser>>(new Mock<IUserStore<ApplicationUser>>().Object, null, null, null, null, null, null, null, null);

            _controller = new BookRequestController(_bookRequestRepositoryMock.Object, _booksRepositoryMock.Object, _clientRepositoryMock.Object, _mapperMock.Object, userManagerMock.Object, _mockUserService.Object);
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
            var clientId = Guid.NewGuid();

            _mockUserService.Setup(s => s.GetUserIdAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ReturnsAsync(clientId);
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
        public async Task AddNewRequest_ReturnsUnauthorized_WhenClientIdIsNull()
        {
            // Arrange
            var bookRequestDto = new BookRequestDto();

            _mockUserService.Setup(s => s.GetUserIdAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ReturnsAsync((Guid?)null);

            // Act
            var result = await _controller.AddNewRequest(bookRequestDto);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedResult>(result.Result);
        }

        [Fact]
        public async Task AddNewRequest_ReturnsStatusCode403_WhenGlobalExceptionThrown()
        {
            // Arrange
            var bookRequestDto = new BookRequestDto();
            var bookRequest = BookRequest.CreatePendingRequest("Title", DateTime.Now, DateTime.Now.AddDays(7), new Client(), new Book());
            var clientId = Guid.NewGuid();


            _mockUserService.Setup(s => s.GetUserIdAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ReturnsAsync(clientId);
            _mapperMock.Setup(m => m.Map<BookRequest>(bookRequestDto)).Returns(bookRequest);
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
            var bookRequest = BookRequest.CreatePendingRequest("Title", DateTime.Now, DateTime.Now.AddDays(7), new Client(), new Book());
            var clientId = Guid.NewGuid();


            _mockUserService.Setup(s => s.GetUserIdAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ReturnsAsync(clientId);
            _mapperMock.Setup(m => m.Map<BookRequest>(bookRequestDto)).Returns(bookRequest);
            _bookRequestRepositoryMock.Setup(repo => repo.AddNewRequest(It.IsAny<BookRequest>())).ThrowsAsync(new Exception("Test exception"));

            // Act & Assert
            await Assert.ThrowsAsync<GlobalException>(() => _controller.AddNewRequest(bookRequestDto));
        }

        [Fact]
        public async Task GetBookRequestsByClient_ReturnsOkResult_WithListOfBookRequestDtos()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var bookRequests = new List<BookRequest> { BookRequest.CreatePendingRequest("Title", DateTime.Now, DateTime.Now.AddDays(7), new Client(), new Book()) };
            var bookRequestDtos = new List<BookRequestDto> { new BookRequestDto() };
           
            _mockUserService.Setup(s => s.GetUserIdAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ReturnsAsync(clientId);
            _bookRequestRepositoryMock.Setup(repo => repo.GetBookRequestsByClientId(clientId)).ReturnsAsync(bookRequests);
            _mapperMock.Setup(m => m.Map<List<BookRequestDto>>(bookRequests)).Returns(bookRequestDtos);

            // Act
            var result = await _controller.GetBookRequestsByClient();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<List<BookRequestDto>>(okResult.Value);
            Assert.Equal(bookRequestDtos, returnValue);
        }

        [Fact]
        public async Task GetBookRequestsByClient_ThrowsGlobalException()
        {
            // Arrange
            var clientId = Guid.NewGuid();

            _mockUserService.Setup(s => s.GetUserIdAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ReturnsAsync(clientId);
            _bookRequestRepositoryMock.Setup(repo => repo.GetBookRequestsByClientId(clientId)).ThrowsAsync(new Exception("Test exception"));

            // Act & Assert
            await Assert.ThrowsAsync<GlobalException>(() => _controller.GetBookRequestsByClient());
        }

        [Fact]
        public async Task GetBookRequestsByClient_ReturnsUnauthorized_WhenClientIdIsNull()
        {
            // Arrange
            _mockUserService.Setup(s => s.GetUserIdAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ReturnsAsync((Guid?)null);

            // Act
            var result = await _controller.GetBookRequestsByClient();

            // Assert
            Assert.IsType<UnauthorizedResult>(result.Result);
        }

        [Fact]
        public async Task CancelRequest_ReturnsOkResult_WithBookRequestDto()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var bookId = Guid.NewGuid();
            var bookRequest = BookRequest.CreatePendingRequest("Title", DateTime.Now, DateTime.Now.AddDays(7), new Client(), new Book());
            var bookRequestDto = new BookRequestDto();

            _mockUserService.Setup(s => s.GetUserIdAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ReturnsAsync(clientId);
            _bookRequestRepositoryMock.Setup(repo => repo.CancelRequest(clientId, bookId)).ReturnsAsync(bookRequest);
            _mapperMock.Setup(m => m.Map<BookRequestDto>(bookRequest)).Returns(bookRequestDto);

            // Act
            var result = await _controller.CancelRequest(bookId);

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

            _mockUserService.Setup(s => s.GetUserIdAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ReturnsAsync(clientId);
            _bookRequestRepositoryMock.Setup(repo => repo.CancelRequest(clientId, bookId)).ThrowsAsync(new GlobalException("Test exception"));

            // Act
            var result = await _controller.CancelRequest(bookId);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(403, statusCodeResult.StatusCode);
            Assert.Equal("Test exception", statusCodeResult.Value);
        }

        [Fact]
        public async Task CancelRequest_ReturnsUnauthorized_WhenClientIdIsNull()
        {
            // Arrange
            var bookId = Guid.NewGuid();

            _mockUserService.Setup(s => s.GetUserIdAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ReturnsAsync((Guid?)null);

            // Act
            var result = await _controller.CancelRequest(bookId);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedResult>(result.Result);
        }

        [Fact]
        public async Task CancelRequest_ThrowsGlobalException()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var bookId = Guid.NewGuid();

            _mockUserService.Setup(s => s.GetUserIdAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ReturnsAsync(clientId);
            _bookRequestRepositoryMock.Setup(repo => repo.CancelRequest(clientId, bookId)).ThrowsAsync(new Exception("Unexpected exception"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<GlobalException>(() => _controller.CancelRequest(bookId));
            Assert.Contains("in BookRequestController", exception.Message);
        }

        [Fact]
        public async Task CancelRequestByClient_ReturnsOkResult_WithBookRequestDto()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var bookId = Guid.NewGuid();
            var bookRequest = BookRequest.CreatePendingRequest("Title", DateTime.Now, DateTime.Now.AddDays(7), new Client(), new Book());
            var bookRequestDto = new BookRequestDto();

            _bookRequestRepositoryMock.Setup(repo => repo.CancelRequest(clientId, bookId)).ReturnsAsync(bookRequest);
            _mapperMock.Setup(m => m.Map<BookRequestDto>(bookRequest)).Returns(bookRequestDto);

            // Act
            var result = await _controller.CancelRequestByClient(bookId, clientId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<BookRequestDto>(okResult.Value);
            Assert.Equal(bookRequestDto, returnValue);
        }

        [Fact]
        public async Task CancelRequestByClient_ReturnsStatusCode403_WhenGlobalExceptionThrown()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var bookId = Guid.NewGuid();

            _bookRequestRepositoryMock.Setup(repo => repo.CancelRequest(clientId, bookId)).ThrowsAsync(new GlobalException("Test exception"));

            // Act
            var result = await _controller.CancelRequestByClient(bookId, clientId);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(403, statusCodeResult.StatusCode);
            Assert.Equal("Test exception", statusCodeResult.Value);
        }

        [Fact]
        public async Task CancelRequestByClient_ThrowsGlobalException()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var bookId = Guid.NewGuid();

            _bookRequestRepositoryMock.Setup(repo => repo.CancelRequest(clientId, bookId)).ThrowsAsync(new Exception("Unexpected exception"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<GlobalException>(() => _controller.CancelRequestByClient(bookId, clientId));
            Assert.Contains("in BookRequestController", exception.Message);
        }

        [Fact]
        public async Task ApproveRequest_ReturnsOkResult_WithBookRequestDto()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var bookId = Guid.NewGuid();
            var bookRequest = BookRequest.CreatePendingRequest("Title", DateTime.Now, DateTime.Now.AddDays(7), new Client(), new Book());
            var bookRequestDto = new BookRequestDto();

            _mockUserService.Setup(s => s.GetUserIdAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ReturnsAsync(clientId);
            _mapperMock.Setup(m => m.Map<BookRequest>(bookRequestDto)).Returns(bookRequest);
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
            var bookRequest = BookRequest.CreatePendingRequest("Title", DateTime.Now, DateTime.Now.AddDays(7), new Client(), new Book());

            _bookRequestRepositoryMock.Setup(repo => repo.ApproveRequest(clientId, bookId)).ThrowsAsync(new GlobalException("Test exception"));
            _mockUserService.Setup(s => s.GetUserIdAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ReturnsAsync(clientId);

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
            var bookRequest = BookRequest.CreatePendingRequest("Title", DateTime.Now, DateTime.Now.AddDays(7), new Client(), new Book());

            _mockUserService.Setup(s => s.GetUserIdAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ReturnsAsync(clientId);
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

            _mockUserService.Setup(s => s.GetUserIdAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ReturnsAsync(clientId);
            _bookRequestRepositoryMock.Setup(repo => repo.ReturnRequest(clientId, bookId)).ReturnsAsync(bookRequest);
            _mapperMock.Setup(m => m.Map<BookRequestDto>(bookRequest)).Returns(bookRequestDto);

            // Act
            var result = await _controller.ReturnRequest(bookId);

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

            _mockUserService.Setup(s => s.GetUserIdAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ReturnsAsync(clientId);
            _bookRequestRepositoryMock.Setup(repo => repo.ReturnRequest(clientId, bookId)).ThrowsAsync(new GlobalException("Test exception"));

            // Act
            var result = await _controller.ReturnRequest(bookId);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(403, statusCodeResult.StatusCode);
            Assert.Equal("Test exception", statusCodeResult.Value);
        }

        [Fact]
        public async Task ReturnRequest_ReturnsUnauthorized_WhenClientIdIsNull()
        {
            // Arrange
            var bookId = Guid.NewGuid();

            _mockUserService.Setup(s => s.GetUserIdAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ReturnsAsync((Guid?)null);

            // Act
            var result = await _controller.ReturnRequest(bookId);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedResult>(result.Result);
        }

        [Fact]
        public async Task ReturnRequest_ThrowsGlobalException()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var bookId = Guid.NewGuid();

            _mockUserService.Setup(s => s.GetUserIdAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ReturnsAsync(clientId);
            _bookRequestRepositoryMock.Setup(repo => repo.ReturnRequest(clientId, bookId)).ThrowsAsync(new Exception("Test exception"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<GlobalException>(() => _controller.ReturnRequest(bookId));
            Assert.Contains("in BookRequestController", exception.Message);
        }

        [Fact]
        public async Task OverdueRequest_ReturnsOkResult_WithBookRequestDto()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var bookId = Guid.NewGuid();
            var bookRequest = BookRequest.CreatePendingRequest("Title", DateTime.Now, DateTime.Now.AddDays(7), new Client(), new Book());
            var bookRequestDto = new BookRequestDto();

            _mockUserService.Setup(s => s.GetUserIdAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ReturnsAsync(clientId);
            _bookRequestRepositoryMock.Setup(repo => repo.OverdueRequest(clientId, bookId)).ReturnsAsync(bookRequest);
            _mapperMock.Setup(m => m.Map<BookRequestDto>(bookRequest)).Returns(bookRequestDto);

            // Act
            var result = await _controller.OverdueRequest(bookId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<BookRequestDto>(okResult.Value);
            Assert.Equal(bookRequestDto, returnValue);
        }

        [Fact]
        public async Task OverdueRequest_ReturnsUnauthorized_WhenClientIdIsNull()
        {
            // Arrange
            var bookId = Guid.NewGuid();
            _mockUserService.Setup(s => s.GetUserIdAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ReturnsAsync((Guid?)null);

            // Act
            var result = await _controller.OverdueRequest(bookId);

            // Assert
            Assert.IsType<UnauthorizedResult>(result.Result);
        }

        [Fact]
        public async Task OverdueRequest_ReturnsStatusCode403_WhenGlobalExceptionThrown()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var bookId = Guid.NewGuid();

            _mockUserService.Setup(s => s.GetUserIdAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ReturnsAsync(clientId);
            _bookRequestRepositoryMock.Setup(repo => repo.OverdueRequest(clientId, bookId)).ThrowsAsync(new GlobalException("Test exception"));

            // Act
            var result = await _controller.OverdueRequest(bookId);

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

            _mockUserService.Setup(s => s.GetUserIdAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ReturnsAsync(clientId);
            _bookRequestRepositoryMock.Setup(repo => repo.OverdueRequest(clientId, bookId)).ThrowsAsync(new Exception("Test exception"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<GlobalException>(() => _controller.OverdueRequest(bookId));
            Assert.Contains("in BookRequestController", exception.Message);
        }
    }
}