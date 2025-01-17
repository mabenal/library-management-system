using lms.Abstractions.Models;
using lms.Abstractions.Data;
using Microsoft.EntityFrameworkCore;
using lms.Abstractions.Interfaces;

namespace lms_server.Repository
{
    public class SQLClientRepository : IClientRepository
    {
        private readonly LmsDbContext dbContext;

        public SQLClientRepository(LmsDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<List<Client>> GellAllClientsAsync()
        {
            return await dbContext.Clients.ToListAsync();
        }

        public async Task<Client> GetClientByID(Guid id)
        {
            return await dbContext.Clients.FindAsync(id);
        }

        public async Task<Client?> UpdateClientDetails(Guid id, Client client)
        {
            var clientToUpdate = await dbContext.Clients.FindAsync(id);

            if (clientToUpdate == null)
            {
                return null;
            }
            else
            {
                clientToUpdate.Name = client.Name;
                clientToUpdate.LastName = client.LastName;
                clientToUpdate.EmailAddress = client.EmailAddress;
                clientToUpdate.Password = client.Password;
                clientToUpdate.Address = client.Address;
                clientToUpdate.PhoneNumber = client.PhoneNumber;
            }

            await dbContext.SaveChangesAsync();
            return clientToUpdate;
        }

        public async Task<Client> DeleteClientAsync(Guid id)
        {
            var client = dbContext.Clients.Find(id);

            if (client == null)
            {
                return null;
            }
            else
            {
                dbContext.Clients.Remove(client);
                await dbContext.SaveChangesAsync();
            }

            return client;
        }

    }
}
