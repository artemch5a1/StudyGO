using StudyGO.Core.Abstractions.Auth;
using StudyGO.infrastructure.Auth;

namespace StudyGO.API.Services;

public partial class ServiceBuilder
{
    private void ConfigureEmailTokenProvider()
    {
        _services.AddScoped<IEmailVerifyTokenProvider, EmailVerifyTokenProvider>();
    }
}