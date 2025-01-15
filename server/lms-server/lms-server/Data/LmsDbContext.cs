using lms.Abstractions.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace lms_server.Data
{
    public class LmsDbContext : IdentityDbContext<Client, IdentityRole<Guid>, Guid>
    {
        public LmsDbContext(DbContextOptions<LmsDbContext> dbContextOptions) : base(dbContextOptions)
        {
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<BookRequest> BookRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
           // modelBuilder.Entity<BookRequest>()
           //.HasOne(br => br.Client)  // Each BookRequest has one Client
           //.WithMany()  // Each Client can have many BookRequests
           //.HasForeignKey(br => br.ClientId)  // Foreign key 'ClientId' in BookRequest
           //.OnDelete(DeleteBehavior.Restrict);  // Optional: Set delete behavior

           // // Configure the BookRequest-Book relationship
           // modelBuilder.Entity<BookRequest>()
           //     .HasOne(br => br.Book)  // Each BookRequest has one Book
           //     .WithMany()  // A Book can have many BookRequests
           //     .HasForeignKey(br => br.BookId)  // Foreign key 'BookId' in BookRequest
           //     .OnDelete(DeleteBehavior.Restrict);  // Optional: Set delete behavior

            var roles = new List<IdentityRole>
            {
                new IdentityRole { Id=Guid.NewGuid().ToString(), Name = "Library Manager" , NormalizedName="Library Manager".ToUpper()},

                new IdentityRole { Id=Guid.NewGuid().ToString(), Name = "Librarian", NormalizedName = "Librarian".ToUpper() },
                new IdentityRole { Id=Guid.NewGuid().ToString(), Name = "Client", NormalizedName = "Client".ToUpper()}
            };

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
            modelBuilder.Entity<IdentityRole>().HasData(roles);
        }
        }
    }

