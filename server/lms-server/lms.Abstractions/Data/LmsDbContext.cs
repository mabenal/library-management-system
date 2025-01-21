using lms.Abstractions.Models;
using lms.Abstractions.Models.DTO;
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

            var bookEntity = new List<Book>()
            {
                new Book()
                {
                    Id = Guid.NewGuid(),
                    Title = "The Alchemist",
                    YearPublished = new DateTime(1988, 1, 1),
                    Author = "Paulo Coelho",
                    Category = "Fiction",
                    NumberOfCopies = 10,
                    ISBN = "978-0-06-231500-7",
                    Description = "The Alchemist follows the journey of an Andalusian shepherd"
                },
                new Book()
                {
                    Id = Guid.NewGuid(),
                    Title = "The Little Prince",
                    YearPublished = new DateTime(1943, 1, 1),
                    Author = "Antoine de Saint-Exupéry",
                    Category = "Fiction",
                    NumberOfCopies = 10,
                    ISBN = "978-0-15-601219-5",
                    Description = "The Little Prince is philosophical tale, with humanist values"
                }
            };




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
                new Client()
                {
                    Id = Guid.NewGuid(),
                    Name = "Sanele",
                    LastName = "Mkhize",
                    EmailAddress = "Sanele.Mkhize@gmail.com",
                    Password = "1234SM",
                    Address = "1234 PL RELX St",
                    PhoneNumber = "123-456-7890"
                }

            };

            modelBuilder.Entity<Book>().HasData(bookEntity);
            modelBuilder.Entity<Client>().HasData(clientEntity);

        }

    }
}


