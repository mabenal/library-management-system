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
        public string Username { get; set; }
        public string CurrentPassword { get; set; }
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }
    }
}
