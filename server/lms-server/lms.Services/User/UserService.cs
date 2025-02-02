using lms.Abstractions.Interfaces;
using lms.Abstractions.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace lms.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IClientRepository clientRepository;

        public UserService(UserManager<ApplicationUser> userManager, IClientRepository clientRepository)
        {
            this.userManager = userManager;
            this.clientRepository = clientRepository;
        }

        public async Task<Guid?> GetUserIdAsync(ClaimsPrincipal user)
        {
            var userEmail = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (userEmail == null)
            {
                return null;
            }

            var client = await clientRepository.GetClientbyEmail(userEmail);
            return client?.Id;
        }
    }
}