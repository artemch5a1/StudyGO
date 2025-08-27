using StudyGO.Core.Abstractions.EmailServices;
using StudyGO.infrastructure.EmailServices;
using StudyGO.infrastructure.SmtpClient;
using StudyGO.infrastructure.SmtpClientFactory.SmtpClient;

namespace StudyGO.API.Services;

public partial class ServiceBuilder
{
    private void ConfigureEmailService()
    {
        _services.Configure<EmailServiceOptions>(_configuration.GetSection("EmailSettings"));

        _services.AddSingleton<ISmtpClientFactory, DefaultSmtpClientFactory>();

        _services.Configure<SmtpClientOptions>(opt => opt.PoolSize = 5);
        
        _services.AddSingleton<ISmtpSender, SmtpClientPool>();
        
        _services.AddScoped<IEmailService, EmailService>();
    }
}