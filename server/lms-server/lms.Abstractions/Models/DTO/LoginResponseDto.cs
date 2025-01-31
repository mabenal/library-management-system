using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lms.Abstractions.Models.DTO
{
    public class LoginResponseDto
    {
        public string? Token { get; set; }

        public  string?  Username { get; set; }

        public required List<string> UserRoles { get; set; }

        //public int ExpiresIn { get; set; }
    }
}
