using lms.Abstractions.Models;

namespace lms_server.Repository
{
    public interface IBooksRepository
    {
        Task<List<Book>> GetAllBooksAsync();

        Task<List<Client>> GellAllClientsAsync();

        Task<Client> GetClientByID(Guid id);

        Task<Client> UpdateClientDetails(Guid id,Client client);

        Task<Client> DeleteClientAsync(Guid id);
    }
}
