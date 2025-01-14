using lms_server.Data;
using lms.Abstractions.Models;
using Microsoft.EntityFrameworkCore;

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
    }
}
