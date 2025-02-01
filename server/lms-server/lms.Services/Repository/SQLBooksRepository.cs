using lms.Abstractions.Data;
using lms.Abstractions.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using lms.Abstractions.Interfaces;
using System.Net;
using lms.Abstractions.Exceptions;
using Microsoft.Extensions.Logging;


namespace lms.Services.Repository
{
    public class SQLBooksRepository : IBooksRepository
    {
        private readonly ILmsDbContext dbContext;
        private readonly ILogger<SQLBooksRepository> _logger;

        public SQLBooksRepository(ILmsDbContext dbContext, ILogger<SQLBooksRepository> logger)
        {
            this.dbContext = dbContext;
            this._logger = logger;
        }
        
        public async Task<List<Book>> GetAllBooksAsync()
        {
            return await dbContext.Books.ToListAsync();
        }
        
        public async Task<Book>GetBookById(Guid id)
        {
            return await dbContext.Books.FindAsync(id);
        }

        public async Task<Book> AddNewBook(Book book)
        {
            //Checkn if the ISBN is already in the database
            var bookExists = await dbContext.Books.FirstOrDefaultAsync(b => b.ISBN == book.ISBN);
            if(bookExists !=null)
            {
                throw new GlobalException("Book with the same ISBN already exists");
            }
            await dbContext.Books.AddAsync(book);
            await dbContext.SaveChangesAsync();

            return book;
        }

        public async Task<List<Book>> SearchBooksAsync(string query)
        {
            try
            {
                var searchTerms = query.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                var booksQuery = dbContext.Books.AsNoTracking().AsQueryable();

                foreach (var term in searchTerms)
                {
                    booksQuery = booksQuery.Where(b => EF.Functions.Like(b.Title, $"%{term}%") ||
                                                       EF.Functions.Like(b.Author, $"%{term}%") ||
                                                       EF.Functions.Like(b.ISBN, $"%{term}%") ||
                                                       EF.Functions.Like(b.Category, $"%{term}%"));
                }

                return await booksQuery.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while searching for books.");
                throw;
            }
        }

        public async Task<Book?> UpdateNewBook(Guid id, Book book)
        {
            var bookToUpdate = await dbContext.Books.FindAsync(id);

            if (bookToUpdate == null)
            {
                return null;
            }

                bookToUpdate.Id = book.Id;
                bookToUpdate.Title = book.Title;
                bookToUpdate.Author = book.Author;
                bookToUpdate.Description = book.Description;
                bookToUpdate.YearPublished = book.YearPublished;
                bookToUpdate.NumberOfCopies = book.NumberOfCopies;
                bookToUpdate.Category = book.Category;
                bookToUpdate.Publisher = book.Publisher;
                bookToUpdate.PageCount = book.PageCount;
                bookToUpdate.Thumbnail = book.Thumbnail;
                bookToUpdate.ISBN = book.ISBN;
            await dbContext.SaveChangesAsync();
            

             return bookToUpdate;
        }

        public async Task<Book?> DeleteBookAsync(Guid id)
        {
            var book = await dbContext.Books.FindAsync(id);

            if (book == null)
            {
                return null;
            }

            dbContext.Books.Remove(book);
            await dbContext.SaveChangesAsync();

            return book;
        }
    }
}
