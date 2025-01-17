using System.ComponentModel.DataAnnotations.Schema;

namespace lms.Abstractions.Models
{
    public class Book
    {
        public Guid Id { get; set; }
        public string? ISBN { get; set; }
        public string? Title { get; set; }
        public string? Author { get; set; }
        public string? YearPublished { get; set; }
        public string? Publisher { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
        public string? Thumbnail { get; set; }
        public int NumberOfCopies { get; set; }
        public int PageCount { get; set; }
    }
}