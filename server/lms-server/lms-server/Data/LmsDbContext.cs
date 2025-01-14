using lms_server.Models;
using Microsoft.EntityFrameworkCore;

namespace lms_server.Data
{
    public class LmsDbContext: DbContext
    {
        public LmsDbContext(DbContextOptions<LmsDbContext> dbContextOptions): base(dbContextOptions)
        {
        }

        DbSet<Book> Books { get; set; }

        DbSet<Client> Clients { get; set; }

        DbSet<BookRequest> BookRequests { get; set; }

    }
}
