using System.ComponentModel.DataAnnotations;

namespace lms.Abstractions.Models.DTO
{
    public class BookDto
    {
        [Required]
        public string? ISBN { get; set; }
        [Required]
        public string? Title { get; set; }
        [Required]
        public string? Author { get; set; }
        [Required]
        public string? YearPublished { get; set; }
        [Required]
        public string? Publisher { get; set; }
        [Required]
        public string? Description { get; set; }
        [Required]
        public string? Category { get; set; }
        [Required]
        public string? Thumbnail { get; set; }
        [Required]
        public int NumberOfCopies { get; set; }
        [Required]
        public int PageCount { get; set; }
    }
}
