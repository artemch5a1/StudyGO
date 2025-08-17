using StudyGO.Core.Abstractions.Verification;
using StudyGO.infrastructure.Extensions;

namespace StudyGO.API.Services;

public partial class ServiceBuilder
{
    private void ConfigureVerificationService()
    {
        _services.AddScoped<IVerificationService, VerificationService>();
    }
}