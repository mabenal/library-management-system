using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lms.Abstractions.Models.DTO
{
    public class ChangePasswordRequestDto
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public required string Username { get; set; }

        [Required]
        public required string CurrentPassword { get; set; }
        
        [DataType(DataType.Password)]
        [Required]
        public required string NewPassword { get; set; }
    }
}
