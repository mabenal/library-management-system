using lms_server.Models;
using Microsoft.EntityFrameworkCore;

namespace lms_server.Data
{
    public class LmsDbContext: DbContext
    {
        public LmsDbContext(DbContextOptions<LmsDbContext> dbContextOptions): base(dbContextOptions)
        {
        }

        public DbSet<Book> Books { get; set; }

        public DbSet<Client> Clients { get; set; }

        public DbSet<BookRequest> BookRequests { get; set; }

    }
}
