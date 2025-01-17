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

            modelBuilder.Entity<Book>()
                .HasKey(b => b.Id);

            var clientEntity = new List<Book>()
            {
                new Book()
                {
                    Id = Guid.NewGuid(),
                    Title = "Zabdile",
                    Author = "Mkhize",
                    YearPublished = "Zamdile.Mkhize@gmail.com",
                    Publisher = "1234ZM",
                    Description = "1234 PL Manzi St",
                    Category = "123-456-7890",
                    Thumbnail = "Zamdile.Mkhize@gmail.com",
                    ISBN = "1234ZM",
                    NumberOfCopies = 2,
                    PageCount = 2
                },
            };

            modelBuilder.Entity<Client>().HasData(clientEntity);
        }
    }
}