using System.ComponentModel.DataAnnotations;

namespace lms.Abstractions.Models.DTO
{
    public class BookDto
    {
        [Required]
        public string? Title { get; set; }
        [Required]
        public DateTime YearPublished { get; set; }
        [Required]
        public string? Author { get; set; }
        public string? Category { get; set; }
        [Required]
        public int NumberOfCopies { get; set; }
        [Required]
        public string? ISBN { get; set; }
        [Required]
        public string? Description { get; set; }
    }
}
