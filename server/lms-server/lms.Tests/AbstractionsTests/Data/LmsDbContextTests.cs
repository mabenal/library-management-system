using Xunit;
using Microsoft.EntityFrameworkCore;
using lms.Abstractions.Data;
using lms.Abstractions.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace lms.Tests.AbstractionsTests.Data
{
    public class LmsDbContextTests
    {
        private DbContextOptions<LmsDbContext> CreateDbContextOptions()
        {
            return new DbContextOptionsBuilder<LmsDbContext>()
                .UseSqlServer("Server=localhost\\SQLEXPRESS;Database=LmsDatabase;Trusted_Connection=True; TrustServerCertificate=True")
                .Options;

        }

        private async Task InitializeDatabase(DbContextOptions<LmsDbContext> options)
        {
            using (var context = new LmsDbContext(options))
            {
                await context.Database.OpenConnectionAsync();
                await context.Database.EnsureCreatedAsync();
            }
        }

        [Fact]
        public async Task CanAddAndRetrieveClient()
        {
            // Arrange
            var options = CreateDbContextOptions();
            await InitializeDatabase(options);

            var client = new Client
            {
                Id = Guid.NewGuid(),
                Name = "Test Client",
                LastName = "Test LastName",
                EmailAddress = "test@example.com",
                Password = "password",
                Address = "123 Test St",
                PhoneNumber = "123-456-7890"
            };

            // Act
            using (var context = new LmsDbContext(options))
            {
                context.Clients.Add(client);
                await context.SaveChangesAsync();
            }

            // Assert
            using (var context = new LmsDbContext(options))
            {
                var retrievedClient = await context.Clients.FindAsync(client.Id);
                Assert.NotNull(retrievedClient);
                Assert.Equal(client.Name, retrievedClient.Name);
            }
        }

        [Fact]
        public async Task CanUpdateClient()
        {
            // Arrange
            var options = CreateDbContextOptions();
            await InitializeDatabase(options);

            var client = new Client
            {
                Id = Guid.NewGuid(),
                Name = "Test Client",
                LastName = "Test LastName",
                EmailAddress = "test@example.com",
                Password = "password",
                Address = "123 Test St",
                PhoneNumber = "123-456-7890"
            };

            using (var context = new LmsDbContext(options))
            {
                context.Clients.Add(client);
                await context.SaveChangesAsync();
            }

            // Act
            using (var context = new LmsDbContext(options))
            {
                var clientToUpdate = await context.Clients.FindAsync(client.Id);
                clientToUpdate.Name = "Updated Client";
                await context.SaveChangesAsync();
            }

            // Assert
            using (var context = new LmsDbContext(options))
            {
                var updatedClient = await context.Clients.FindAsync(client.Id);
                Assert.NotNull(updatedClient);
                Assert.Equal("Updated Client", updatedClient.Name);
            }
        }

        [Fact]
        public async Task CanDeleteClient()
        {
            // Arrange
            var options = CreateDbContextOptions();
            await InitializeDatabase(options);

            var client = new Client
            {
                Id = Guid.NewGuid(),
                Name = "Test Client",
                LastName = "Test LastName",
                EmailAddress = "test@example.com",
                Password = "password",
                Address = "123 Test St",
                PhoneNumber = "123-456-7890"
            };

            using (var context = new LmsDbContext(options))
            {
                context.Clients.Add(client);
                await context.SaveChangesAsync();
            }

            // Act
            using (var context = new LmsDbContext(options))
            {
                var clientToDelete = await context.Clients.FindAsync(client.Id);
                context.Clients.Remove(clientToDelete);
                await context.SaveChangesAsync();
            }

            // Assert
            using (var context = new LmsDbContext(options))
            {
                var deletedClient = await context.Clients.FindAsync(client.Id);
                Assert.Null(deletedClient);
            }
        }

        [Fact]
        public async Task CanRetrieveSeededClients()
        {
            // Arrange
            var options = CreateDbContextOptions();
            await InitializeDatabase(options);

            // Act & Assert
            using (var context = new LmsDbContext(options))
            {
                var clients = await context.Clients.ToListAsync();
                Assert.Contains(clients, c => c.Name == "Zabdile");
                Assert.Contains(clients, c => c.Name == "Sanele");
            }
        }
    }
}