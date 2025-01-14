namespace lms_server.Models
{
    public class Book
    {
        public Guid Id { get; set; }

        public required  string Title { get; set; }

        public DateTime YearPublished  { get; set; }

        public string Author { get; set; }

        public string? Category { get; set; }

        public int NumberOfCopies { get; set; }

        public string ISBN { get; set; }

        public string Description { get; set; }
    }
}
