using lms_server.Data;
using lms.Abstractions.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

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
        
        public async Task<Book>GetBookById(Guid id)
        {
            return await dbContext.Books.FindAsync(id);
        }

       public async Task<Book> AddNewBook(Book book)
        {
            await dbContext.Books.AddAsync(book);
            await dbContext.SaveChangesAsync();

            return book;
        }

        public async Task<Book?> UpdateNewBook(Guid id, Book book)
        {
            var bookToUpdate = await dbContext.Books.FindAsync(id);

            if (bookToUpdate == null)
            {
                return null;
            }

                bookToUpdate.Title = book.Title;
                bookToUpdate.Author = book.Author;
                bookToUpdate.Description = book.Description;
                bookToUpdate.ISBN = book.ISBN;
                bookToUpdate.YearPublished = book.YearPublished;
                bookToUpdate.NumberOfCopies = book.NumberOfCopies;
                bookToUpdate.Category = book.Category;
                await dbContext.SaveChangesAsync();
            

             return bookToUpdate;
        }

        public async Task<Book?> DeleteBookAsync(Guid id)
        {
            var book = dbContext.Books.Find(id);

            if (book == null) {

                return null;

            }
            dbContext.Remove(book);
           await dbContext.SaveChangesAsync();

            return book;
           
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
