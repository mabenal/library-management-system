using Microsoft.AspNetCore.Identity;

namespace lms.Abstractions.Models.DTO
{
    public class AccountActionResponseDto
    {
        public bool isSuccessful { get; set; }

        public  IEnumerable<IdentityError>? errors { get; set; }
    }
}
