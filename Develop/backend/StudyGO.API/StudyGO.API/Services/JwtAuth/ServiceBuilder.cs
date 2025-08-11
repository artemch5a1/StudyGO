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
        private void ConfigureJwtAuthentication()
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
                    policy => policy.RequireRole(RolesEnum.Admin.GetString())
                )
                .AddPolicy(
                    PolicyNames.UserOrAdmin,
                    policy =>
                        policy.RequireRole(RolesEnum.User.GetString(), RolesEnum.Admin.GetString())
                )
                .AddPolicy(
                    PolicyNames.TutorOrAdmin,
                    policy =>
                        policy.RequireRole(RolesEnum.Tutor.GetString(), RolesEnum.Admin.GetString())
                )
                .AddPolicy(
                    PolicyNames.TutorOnly,
                    policy => policy.RequireRole(RolesEnum.Tutor.GetString())
                )
                .AddPolicy(
                    PolicyNames.UserOnly,
                    policy => policy.RequireRole(RolesEnum.User.GetString())
                );
            ;
        }
    }
}
