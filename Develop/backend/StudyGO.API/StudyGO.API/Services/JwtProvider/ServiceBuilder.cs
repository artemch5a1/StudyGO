using StudyGO.Core.Abstractions.Auth;
using StudyGO.infrastructure.Auth;

namespace StudyGO.API.Services
{
    public partial class ServiceBuilder
    {
        private void ConfigureJwtProvider()
        {
            _services.Configure<JwtOptions>(opt => {
                opt.Issuer = _configuration["Jwt:Issuer"]!;
                opt.Key = _configuration["Jwt:key"]!;
                opt.ExpiresMinutes = int.TryParse(_configuration["Jwt:Expires"], out int m) ? m : 120;
            });

            _services.AddScoped<IJwtTokenProvider, JwtTokenProvider>();
        }
    }
}
