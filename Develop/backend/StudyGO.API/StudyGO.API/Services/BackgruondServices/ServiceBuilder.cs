using System.Threading.Channels;
using StudyGO.API.BackgroundServices;
using StudyGO.Contracts.Contracts;

namespace StudyGO.API.Services;

public partial class ServiceBuilder
{
    private void ConfigureBackgroundServices()
    {
        _services.AddSingleton(Channel.CreateUnbounded<VerificationJob>());
        _services.AddHostedService<EmailVerificationWorker>();
    }
}