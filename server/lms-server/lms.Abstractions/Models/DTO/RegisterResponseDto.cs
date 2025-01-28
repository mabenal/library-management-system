using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lms.Abstractions.Models.DTO
{
    public class RegisterResponseDto
    {
        public bool? Succeeded { get; set; }

        public IEnumerable<IdentityError> Errors { get; set; } = Enumerable.Empty<IdentityError>();

    }
}

