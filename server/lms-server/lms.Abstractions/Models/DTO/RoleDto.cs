using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lms.Abstractions.Models.DTO
{
    public class RoleDto
    {
        [Required]
        public required Guid UserId { get; set; }
        [Required]
        public required string Role { get; set; }
    }
}
