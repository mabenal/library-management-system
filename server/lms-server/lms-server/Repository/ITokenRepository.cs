using lms.Abstractions.Models;

namespace lms_server.Repository
{
    public interface ITokenRepository
    {
        string CreateJWTToken(Client user);
    }
}
