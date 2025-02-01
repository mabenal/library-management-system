using Xunit;
using Moq;
using lms.Abstractions.Data;
using lms.Abstractions.Models;
using lms_server.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using lms.Abstractions.Interfaces;

namespace lms.Tests.ServicesTests.Repository
{
    public class SQLClientRepositoryTests
    {
        private readonly Mock<ILmsDbContext> _dbContextMock;
        private readonly SQLClientRepository _repository;

        public SQLClientRepositoryTests()
        {
            _dbContextMock = new Mock<ILmsDbContext>();
            _repository = new SQLClientRepository(_dbContextMock.Object);
        }

        [Fact]
        public async Task GetAllClientsAsync_ShouldReturnClients()
        {
            // Arrange
            var clients = new List<Client> { new Client { Id = Guid.NewGuid(), Name = "Test Client" } }.AsQueryable();
            var dbSetMock = CreateDbSetMock(clients);

            _dbContextMock.Setup(db => db.Clients).Returns(dbSetMock.Object);

            // Act
            var result = await _repository.GetAllClientsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
        }

        [Fact]
        public async Task GetClientById_ShouldReturnClient_WhenClientExists()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var client = new Client { Id = clientId, Name = "Test Client" };

            _dbContextMock.Setup(db => db.Clients.FindAsync(clientId)).ReturnsAsync(client);

            // Act
            var result = await _repository.GetClientById(clientId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(clientId, result.Id);
        }

        [Fact]
        public async Task GetClientById_ShouldReturnNull_WhenClientDoesNotExist()
        {
            // Arrange
            var clientId = Guid.NewGuid();

            _dbContextMock.Setup(db => db.Clients.FindAsync(clientId)).ReturnsAsync((Client)null);

            // Act
            var result = await _repository.GetClientById(clientId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateClientDetails_ShouldUpdateClient_WhenClientExists()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var client = new Client { Id = clientId, Name = "Updated Client" };
            var existingClient = new Client { Id = clientId, Name = "Existing Client" };

            _dbContextMock.Setup(db => db.Clients.FindAsync(clientId)).ReturnsAsync(existingClient);

            // Act
            var result = await _repository.UpdateClientDetails(clientId, client);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Updated Client", result.Name);
            _dbContextMock.Verify(db => db.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task UpdateClientDetails_ShouldReturnNull_WhenClientDoesNotExist()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var client = new Client { Id = clientId, Name = "Updated Client" };

            _dbContextMock.Setup(db => db.Clients.FindAsync(clientId)).ReturnsAsync((Client)null);

            // Act
            var result = await _repository.UpdateClientDetails(clientId, client);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteClientAsync_ShouldDeleteClient_WhenClientExists()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var client = new Client { Id = clientId, Name = "Test Client" };

            _dbContextMock.Setup(db => db.Clients.FindAsync(clientId)).ReturnsAsync(client);

            // Act
            var result = await _repository.DeleteClientAsync(clientId);

            // Assert
            Assert.NotNull(result);
            _dbContextMock.Verify(db => db.Clients.Remove(client), Times.Once);
            _dbContextMock.Verify(db => db.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task DeleteClientAsync_ShouldReturnNull_WhenClientDoesNotExist()
        {
            // Arrange
            var clientId = Guid.NewGuid();

            _dbContextMock.Setup(db => db.Clients.FindAsync(clientId)).ReturnsAsync((Client)null);

            // Act
            var result = await _repository.DeleteClientAsync(clientId);

            // Assert
            Assert.Null(result);
        }

        private static Mock<DbSet<T>> CreateDbSetMock<T>(IQueryable<T> elements) where T : class
        {
            var dbSetMock = new Mock<DbSet<T>>();
            dbSetMock.As<IQueryable<T>>().Setup(m => m.Provider).Returns(elements.Provider);
            dbSetMock.As<IQueryable<T>>().Setup(m => m.Expression).Returns(elements.Expression);
            dbSetMock.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(elements.ElementType);
            dbSetMock.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(elements.GetEnumerator());
            dbSetMock.As<IAsyncEnumerable<T>>().Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>())).Returns(new TestAsyncEnumerator<T>(elements.GetEnumerator()));
            return dbSetMock;
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
    }
}