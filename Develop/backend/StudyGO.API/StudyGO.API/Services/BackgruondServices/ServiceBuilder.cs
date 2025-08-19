using System.Threading.Channels;
using StudyGO.API.BackgroundServices;
using StudyGO.Contracts.Contracts;
using StudyGO.Core.Abstractions.Contracts;
using StudyGO.infrastructure.Extensions.VerificationJobQueue;

namespace StudyGO.API.Services;

public partial class ServiceBuilder
{
    private void ConfigureBackgroundServices()
    {
        _services.AddSingleton(Channel.CreateUnbounded<VerificationJob>());

        _services.AddSingleton<IVerificationJobQueue, InMemoryVerificationJobQueue>();
        
        _services.AddHostedService<EmailVerificationWorker>();
    }
}