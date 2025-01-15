using Microsoft.AspNetCore.Identity;

namespace lms.Abstractions.Models
{
    public class Client: IdentityUser<Guid>
    {

        public string? Name { get; set; }

        public string? LastName { get; set; }

        public string? Address { get; set; }

        public string? PhoneNumber { get; set; }
    }
}
