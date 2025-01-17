using lms.Abstractions.Models;

namespace lms.Abstractions.Interfaces
{
    public interface IClientRepository
    {
        Task<List<Client>> GellAllClientsAsync();
        Task<Client> GetClientByID(Guid id);
        Task<Client> UpdateClientDetails(Guid id, Client client);
        Task<Client> DeleteClientAsync(Guid id);
    }
}
