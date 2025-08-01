using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using StudyGO.Contracts.Contracts;
using StudyGO.Core.Abstractions.Auth;

namespace StudyGO.infrastructure.Auth
{
    public class JwtTokenProvider : IJwtTokenProvider
    {
        private readonly IOptions<JwtOptions> _options;

        public JwtTokenProvider(IOptions<JwtOptions> options)
        {
            _options = options;
        }

        public string GenerateToken(UserLoginResponse user)
        {
            string key = _options.Value.Key;
            string issuer = _options.Value.Issuer;
            int expiresMinutes = _options.Value.ExpiresMinutes;

            List<Claim> claims =
            [
                new Claim(ClaimTypes.NameIdentifier, user.id.ToString()),
                new Claim(ClaimTypes.Role, user.Role),
            ];

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiresMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
