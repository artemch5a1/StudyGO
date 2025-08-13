using StudyGO.Core.Abstractions.EmailServices;
using StudyGO.infrastructure.RegistryVerification;

namespace StudyGO.API.Services;

public partial class ServiceBuilder
{
    private void ConfigureEmailService()
    {
        _services.Configure<EmailServiceOptions>(_configuration.GetSection("EmailSettings"));

        _services.AddScoped<IEmailService, EmailService>();
    }
}