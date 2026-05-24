using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace PMS.API.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static bool TryGetCurrentUserId(this ClaimsPrincipal user, out Guid userId)
        {
            var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? user.FindFirstValue(JwtRegisteredClaimNames.Sub)
                ?? user.FindFirstValue("sub");

            return Guid.TryParse(userIdClaim, out userId);
        }
    }
}
