using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lms.Abstractions.Models.DTO
{
    public class AssignRoleDto
    {
        public Guid UserId { get; set; }
        public string Role { get; set; }
    }
}
