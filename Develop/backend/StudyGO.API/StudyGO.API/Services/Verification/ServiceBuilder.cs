using StudyGO.API.Options;
using StudyGO.Core.Abstractions.Verification;
using StudyGO.infrastructure.Extensions;

namespace StudyGO.API.Services;

public partial class ServiceBuilder
{
    private void ConfigureVerificationService()
    {
        _services.Configure<EmailConfirmationOptions>(_configuration.GetSection("EmailConfirmation"));
        
        _services.AddScoped<IVerificationService, VerificationService>();
    }
}