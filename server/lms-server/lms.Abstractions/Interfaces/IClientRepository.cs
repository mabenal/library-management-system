using lms.Abstractions.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace lms.Abstractions.Interfaces
{
    public interface IClientRepository
    {
        Task<List<Client>> GetAllClientsAsync();
        Task<Client?> GetClientById(Guid id);
        Task<Client?> UpdateClientDetails(Guid id, Client client);
        Task<Client?> DeleteClientAsync(Guid id);
    }
}