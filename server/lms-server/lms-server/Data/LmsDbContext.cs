using lms_server.Models;
using Microsoft.EntityFrameworkCore;

namespace lms_server.Data
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

                new Book() {
                    Id = Guid.NewGuid(),
                    Title = "The Great Gatsby",
                    Author = "F. Scott Fitzgerald",
                    Category = "Fiction",
                    Description = "The Great Gatsby is a novel by American author F. Scott Fitzgerald. The story takes place in 1922, during the Roaring Twenties, a time of prosperity in the United States after World War I. The book received critical acclaim and is widely regarded as a classic of American literature.",
                    ISBN = "9780743273565",
                    NumberOfCopies = 5,
                    YearPublished = new DateTime(1925, 04, 10)
                },

                new Book(){
                     Id = Guid.NewGuid(),
                    Title = "To Kill a Mockingbird",
                    Author = "Harper Lee",
                    Category = "Fiction",
                    Description = "To Kill",
                    ISBN="jdjdjdjdjdjjjjd",
                    NumberOfCopies = 5,
                    YearPublished= new DateTime(1960, 07, 11) }

              };

            modelBuilder.Entity<Book>().HasData(bookEntity);

        }


        }
    }

