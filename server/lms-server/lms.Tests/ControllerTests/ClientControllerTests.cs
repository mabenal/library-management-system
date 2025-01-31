using Xunit;
using Moq;
using AutoMapper;
using lms.Abstractions.Interfaces;
using lms.Abstractions.Models;
using lms.Abstractions.Models.DTO;
using lms_server.Controllers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using lms.Abstractions.Exceptions;

namespace lms.Tests.ControllerTests
{
    public class ClientsControllerTests
    {
        private readonly Mock<IClientRepository> _clientsRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly ClientController _controller;

        public ClientsControllerTests()
        {
            _clientsRepositoryMock = new Mock<IClientRepository>();
            _mapperMock = new Mock<IMapper>();
            _controller = new ClientController(_clientsRepositoryMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task GetAllClients_ReturnsOkResult_WithListOfClientDtos()
        {
            // Arrange
            var clients = new List<Client> { new Client() };
            var clientDtos = new List<ClientDto> { new ClientDto() };

            _clientsRepositoryMock.Setup(repo => repo.GetAllClientsAsync()).ReturnsAsync(clients);
            _mapperMock.Setup(m => m.Map<List<ClientDto>>(clients)).Returns(clientDtos);

            // Act
            var result = await _controller.GetAllClients();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<List<ClientDto>>(okResult.Value);
            Assert.Equal(clientDtos, returnValue);
        }

        [Fact]
        public async Task GetAllClients_ThrowsGlobalException()
        {
            // Arrange
            _clientsRepositoryMock.Setup(repo => repo.GetAllClientsAsync()).ThrowsAsync(new Exception("Test exception"));

            // Act & Assert
            await Assert.ThrowsAsync<GlobalException>(() => _controller.GetAllClients());
        }

        [Fact]
        public async Task GetClientById_ReturnsOkResult_WithClientDto()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var client = new Client();
            var clientDto = new ClientDto();

            _clientsRepositoryMock.Setup(repo => repo.GetClientById(clientId)).ReturnsAsync(client);
            _mapperMock.Setup(m => m.Map<ClientDto>(client)).Returns(clientDto);

            // Act
            var result = await _controller.GetClientByID(clientId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<ClientDto>(okResult.Value);
            Assert.Equal(clientDto, returnValue);
        }

        [Fact]
        public async Task GetClientById_ThrowsException()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            _clientsRepositoryMock.Setup(repo => repo.GetClientById(clientId)).ThrowsAsync(new Exception("Test exception"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _controller.GetClientByID(clientId));
        }

        [Fact]
        public async Task UpdateClientDetails_ReturnsOkResult_WithClientDto()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var clientDto = new ClientDto();
            var client = new Client();

            _mapperMock.Setup(m => m.Map<Client>(clientDto)).Returns(client);
            _clientsRepositoryMock.Setup(repo => repo.UpdateClientDetails(clientId, client)).ReturnsAsync(client);
            _mapperMock.Setup(m => m.Map<ClientDto>(client)).Returns(clientDto);

            // Act
            var result = await _controller.UpdateClientDetails(clientId, clientDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<ClientDto>(okResult.Value);
            Assert.Equal(clientDto, returnValue);
        }

        [Fact]
        public async Task UpdateClientDetails_ThrowsGlobalException()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var clientDto = new ClientDto();
            _mapperMock.Setup(m => m.Map<Client>(clientDto)).Throws(new Exception("Test exception"));

            // Act & Assert
            await Assert.ThrowsAsync<GlobalException>(() => _controller.UpdateClientDetails(clientId, clientDto));
        }

        [Fact]
        public async Task DeleteClient_ReturnsOkResult_WithClientDto()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var client = new Client();
            var clientDto = new ClientDto();

            _clientsRepositoryMock.Setup(repo => repo.DeleteClientAsync(clientId)).ReturnsAsync(client);
            _mapperMock.Setup(m => m.Map<ClientDto>(client)).Returns(clientDto);

            // Act
            var result = await _controller.DeleteClient(clientId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<ClientDto>(okResult.Value);
            Assert.Equal(clientDto, returnValue);
        }

        [Fact]
        public async Task DeleteClient_ThrowsGlobalException()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            _clientsRepositoryMock.Setup(repo => repo.DeleteClientAsync(clientId)).ThrowsAsync(new Exception("Test exception"));

            // Act & Assert
            await Assert.ThrowsAsync<GlobalException>(() => _controller.DeleteClient(clientId));
        }
    }
}