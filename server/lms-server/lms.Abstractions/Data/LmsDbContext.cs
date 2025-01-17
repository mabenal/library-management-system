using lms.Abstractions.Models;
using Microsoft.EntityFrameworkCore;

namespace lms.Abstractions.Data
{
    public class LmsDbContext : DbContext
    {
        public LmsDbContext(DbContextOptions<LmsDbContext> dbContextOptions) : base(dbContextOptions)
        {
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<BookRequest> BookRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            var clientEntity = new List<Client>()
            {
                new Client()
                {
                    Id = Guid.NewGuid(),
                    Name = "Zabdile",
                    LastName = "Mkhize",
                    EmailAddress = "Zamdile.Mkhize@gmail.com",
                    Password = "1234ZM",
                    Address = "1234 PL Manzi St",
                    PhoneNumber = "123-456-7890"
                },

            };

            modelBuilder.Entity<Client>().HasData(clientEntity);
        }

    }
}


