using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lms.Abstractions.Models.DTO
{
    public class ChangePasswordRequestDto
    {
        public string Username  { get; set; }

        public string CurrentPassword { get; set; }

        public string NewPassword { get; set; }
    }
}
