
using lms.Abstractions.Models;
using lms.Abstractions.Models.DTO;

namespace lms.Abstractions.Interfaces
{
    public interface IAccountRepository
    {
       Task<ApplicationUser> RegisterAsync(ApplicationUser user, string password);

        Task<ApplicationUser> LoginAsync(LoginDto loginDtoObject);
      
    }
}
