﻿using System.ComponentModel.DataAnnotations;

namespace lms.Abstractions.Models.DTO
{
    public class RegisterDto
    {
        [Required]
        [Range(1, 255, ErrorMessage = "Please enter name between 1 and 255 characters")]
        public required string Name { get; set; }
        [Required]
        [Range(1, 255, ErrorMessage = "Please enter last name between 1 and 255 characters")]

        public required string LastName { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        public required string Email { get; set; }

        [Required]
        public required string Address { get; set; }

        [Required]
        [MinLength(10, ErrorMessage ="Phone number must be exactly 10 digits")]
        [MaxLength(10, ErrorMessage = "Phone number must be exactly 10 digits")]

        public required string PhoneNumber { get; set; }

        [Required]
        [DataType(DataType.Password)]

        public required string Password { get; set; }

        [Required]
        public required string ConfirmPassword { get; set; }
    }
}
