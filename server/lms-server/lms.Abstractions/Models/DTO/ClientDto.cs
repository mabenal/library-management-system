using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lms.Abstractions.Models.DTO
{
    public class ClientDto
    {

        public string? Name { get; set; }

        public string? LastName { get; set; }

        public string? EmailAddress { get; set; }

        public string? Password { get; set; }

        public string? Address { get; set; }

        public string? PhoneNumber { get; set; }
    }
}
