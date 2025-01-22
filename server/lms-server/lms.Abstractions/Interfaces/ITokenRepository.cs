using lms.Abstractions.Models;


namespace lms.Abstractions.Interfaces
{
    public interface ITokenRepository
    {
        Task<string> CreateJWTTokenAsync(ApplicationUser user);
    }
}
