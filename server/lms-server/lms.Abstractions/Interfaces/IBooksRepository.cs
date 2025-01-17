using lms.Abstractions.Models;

namespace lms.Abstractions.Interfaces
{
    public interface IBooksRepository
    {
        Task<List<Book>> GetAllBooksAsync();

        Task<Book?> GetBookById(Guid id);

        Task<Book> AddNewBook(Book book);

        Task<Book?> UpdateNewBook(Guid id, Book book);

        Task<Book?> DeleteBookAsync(Guid id);

    }
}
