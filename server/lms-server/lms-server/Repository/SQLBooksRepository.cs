using lms_server.Data;
using lms.Abstractions.Models;
using Microsoft.EntityFrameworkCore;

namespace lms_server.Repository
{
    public class SQLBooksRepository : IBooksRepository
    {
        private readonly LmsDbContext dbContext;

        public SQLBooksRepository(LmsDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<List<Book>> GetAllBooksAsync()
        {
            return await dbContext.Books.ToListAsync();
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

            if(clientToUpdate == null)
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

            if(client == null)
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
