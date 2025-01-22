using lms.Abstractions.Models;


namespace lms.Abstractions.Interfaces
{
    public interface ITokenRepository
    {
        string CreateJWTToken(ApplicationUser user);
    }
}
