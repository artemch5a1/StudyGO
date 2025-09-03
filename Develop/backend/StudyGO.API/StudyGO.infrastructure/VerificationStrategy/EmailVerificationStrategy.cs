using Microsoft.Extensions.Logging;
using StudyGO.Contracts;
using StudyGO.Contracts.Contracts;
using StudyGO.Core.Abstractions.Contracts;
using StudyGO.Core.Abstractions.VerificationStrategy;

namespace StudyGO.infrastructure.VerificationStrategy;

public class EmailVerificationStrategy : IVerificationStrategy
{
    private readonly IVerificationJobQueue _queue;
    private readonly ILogger<EmailVerificationStrategy> _logger;

    public EmailVerificationStrategy(
        IVerificationJobQueue queue,
        ILogger<EmailVerificationStrategy> logger)
    {
        _queue = queue;
        _logger = logger;
    }
    
    
    public RegistryScheme Scheme => RegistryScheme.VerifiedByLink;
    
    public async Task VerifyAsync(RegisteredInformation @event, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Email-верификация для пользователя {UserId}", @event.UserId);
        var job = new VerificationJob(@event.UserId, @event.ConfirmEndpoint, @event.Role.ToString());
        await _queue.EnqueueAsync(job, cancellationToken);
    }
}