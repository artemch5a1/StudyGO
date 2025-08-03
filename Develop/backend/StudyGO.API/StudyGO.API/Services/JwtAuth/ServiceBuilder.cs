using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using StudyGO.API.Enums;
using StudyGO.Core.Enums;
using StudyGO.Core.Extensions;

namespace StudyGO.API.Services
{
    public partial class ServiceBuilder
    {
        public void ConfigureJwtAuthentication()
        {
            _services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(
                    JwtBearerDefaults.AuthenticationScheme,
                    options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = false,
                            ValidateAudience = false,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = new SymmetricSecurityKey(
                                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!)
                            ),
                            RoleClaimType = ClaimTypes.Role,
                        };
                    }
                );

            _services
                .AddAuthorizationBuilder()
                .AddPolicy(
                    PolicyNames.AdminOnly,
                    policy => policy.RequireRole(RolesEnum.admin.GetString())
                )
                .AddPolicy(
                    PolicyNames.UserOrAdmin,
                    policy =>
                        policy.RequireRole(RolesEnum.user.GetString(), RolesEnum.admin.GetString())
                )
                .AddPolicy(
                    PolicyNames.TutorOrAdmin,
                    policy =>
                        policy.RequireRole(RolesEnum.tutor.GetString(), RolesEnum.admin.GetString())
                )
                .AddPolicy(
                    PolicyNames.TutorOnly,
                    policy => policy.RequireRole(RolesEnum.tutor.GetString())
                )
                .AddPolicy(
                    PolicyNames.UserOnly,
                    policy => policy.RequireRole(RolesEnum.user.GetString())
                );
            ;
        }
    }
}
