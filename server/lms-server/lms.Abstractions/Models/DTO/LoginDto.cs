using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace lms.Abstractions.Models.DTO
{
    public class LoginDto
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public required string UserName { get; set; }
        [Required]
        public required string  Password { get; set; }


    }
}
