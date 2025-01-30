using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lms.Abstractions.Models.DTO
{
    public class BookRequestDto
    {
        [Required]
        public string? Status { get; set; }
        public string? Title { get; set; }
        public DateTime DateApproved { get; set; }
        [Required]
        public DateTime DateRequested { get; set; }
        public  DateTime AcceptedReturnDate { get; set; }
        public DateTime DateReturned { get; set; }

        [Required]
        public Guid ClientId { get; set; }
        [Required]
        public Guid BookId { get; set; }


    }
}
