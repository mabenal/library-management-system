using lms_server.Models;
using lms_server.Models.DTO;

namespace lms_server.Repository
{
    public interface IBooksRepository
    {
        Task<List<Book>> GetAllBooksAsync();

        Task<Book?> GetBookById(Guid id);

        Task<Book> AddNewBook(Book book);

        Task<Book?> UpdateNewBook(Guid id, Book book);


    }
}
