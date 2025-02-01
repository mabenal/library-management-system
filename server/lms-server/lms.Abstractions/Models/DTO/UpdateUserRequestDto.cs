using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lms.Abstractions.Models.DTO
{
    public class UpdateUserRequestDto
    {
        [Required]
        [StringLength(255, MinimumLength = 1, ErrorMessage = "Please enter name between 1 and 255 characters")]
        public required string Name { get; set; }
        [Required]
        [StringLength(255, MinimumLength = 1, ErrorMessage = "Please enter name between 1 and 255 characters")]

        public required string LastName { get; set; }
        [Required]
        public required string Address { get; set; }


        [MinLength(10, ErrorMessage = "Phone number must be exactly 10 digits")]
        [MaxLength(10, ErrorMessage = "Phone number must be exactly 10 digits")]

        public required string PhoneNumber { get; set; }

        
    }
}
