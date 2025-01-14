using lms_server.Models;

namespace lms_server.Repository
{
    public interface IBooksRepository
    {

        Task<List<Book>> GetAllBooksAsync();
    }
}
