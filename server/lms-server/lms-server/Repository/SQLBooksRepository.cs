﻿using lms_server.Data;
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
    }
}
