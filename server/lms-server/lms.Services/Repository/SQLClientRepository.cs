using lms.Abstractions.Data;
using lms.Abstractions.Interfaces;
using lms.Abstractions.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace lms_server.Repository
{
    public class SQLClientRepository : IClientRepository
    {
        private readonly ILmsDbContext dbContext;

        public SQLClientRepository(ILmsDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<List<Client>> GetAllClientsAsync()
        {
            return await dbContext.Clients.ToListAsync();
        }

        public async Task<Client?> GetClientById(Guid id)
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

            clientToUpdate.Name = client.Name;
            clientToUpdate.LastName = client.LastName;
            clientToUpdate.EmailAddress = client.EmailAddress;
            clientToUpdate.Password = client.Password;
            clientToUpdate.Address = client.Address;
            clientToUpdate.PhoneNumber = client.PhoneNumber;

            await dbContext.SaveChangesAsync();
            return clientToUpdate;
        }

        public async Task<Client?> DeleteClientAsync(Guid id)
        {
            var clientToDelete = await dbContext.Clients.FindAsync(id);

            if (clientToDelete == null)
            {
                return null;
            }

            dbContext.Clients.Remove(clientToDelete);
            await dbContext.SaveChangesAsync();
            return clientToDelete;
        }
    }
}