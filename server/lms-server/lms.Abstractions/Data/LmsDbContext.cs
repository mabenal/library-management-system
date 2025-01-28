using lms.Abstractions.Models;
using lms.Abstractions.Models.DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace lms.Abstractions.Data
{
    public class LmsDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
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
            modelBuilder.Entity<Client>().HasData(clientEntity);

        }
    }
}