using MediatR;
using Microsoft.Extensions.Logging;
using StudyGO.Contracts;
using StudyGO.Contracts.Contracts;
using StudyGO.Core.Abstractions.VerificationStrategy;

namespace StudyGO.Application.UseCases.Subscribers.RegisteredEvent.Handlers;

public class VerificationHandler : INotificationHandler<RegisteredEvent>
{
    private readonly IVerificationStrategyResolver _resolver;
    private readonly ILogger<VerificationHandler> _logger;

    public VerificationHandler(
        IVerificationStrategyResolver resolver,
        ILogger<VerificationHandler> logger)
    {
        _resolver = resolver;
        _logger = logger;
    }
    
    public async Task Handle(RegisteredEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Обработка регистрации пользователя {UserId} со схемой {Scheme}", 
            notification.Information.UserId, notification.Information.Scheme);
        
        var strategy = GetVerificationStrategy(notification.Information);
        await strategy.VerifyAsync(notification.Information, cancellationToken);
    }
    
    private IVerificationStrategy GetVerificationStrategy(RegisteredInformation information)
    {
        if (!information.RequiredVerification)
        {
            _logger.LogDebug("Верификация не требуется, использование default стратегии");
            return _resolver.Resolve(RegistryScheme.DefaultVerified);
        }
    
        _logger.LogDebug("Использование стратегии {Scheme} для верификации", information.Scheme);
        return _resolver.Resolve(information.Scheme);
    }
}