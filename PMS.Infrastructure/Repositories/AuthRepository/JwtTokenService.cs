using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace PMS.Infrastructure.Repositories.Auth
{
    public class JwtTokenService : Application.Interfaces.IJwtTokenService
    {
        private readonly JwtOptions _options;

        public JwtTokenService(IOptions<JwtOptions> options)
        {
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }

        public string CreateToken(Guid userId, string userName, IEnumerable<Claim>? additionalClaims = null)
        {
            if (string.IsNullOrEmpty(_options.SecretKey)) throw new InvalidOperationException("JWT secret is not configured");

            var expiry = _options.ExpireMinutes > 0 ? _options.ExpireMinutes : 60;

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, userName)
            };

            if (additionalClaims != null)
            {
                claims.AddRange(additionalClaims);
            }

            var keyBytes = Encoding.UTF8.GetBytes(_options.SecretKey);
            var creds = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _options.Issuer,
                audience: _options.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiry),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
