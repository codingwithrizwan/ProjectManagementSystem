using System.Security.Claims;

namespace PMS.Application.Interfaces
{
    public interface IJwtTokenService
    {
        string CreateToken(Guid userId, string userName, IEnumerable<Claim>? additionalClaims = null);
    }
}
