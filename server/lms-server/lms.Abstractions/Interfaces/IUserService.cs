using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace lms.Abstractions.Interfaces
{
    public interface IUserService
    {
        Task<Guid?> GetUserIdAsync(ClaimsPrincipal user);
    }
}
