using Microsoft.AspNetCore.Identity;

namespace lms.Abstractions.Models
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string? Name { get; set; }

        public string? LastName { get; set; }

        public string? Address { get; set; }
    }
}
