using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lms.Abstractions.Models
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string? Name { get; set; }

        public string? LastName { get; set; }

        public string? Address { get; set; }
    }
}
