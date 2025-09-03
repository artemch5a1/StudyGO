using StudyGO.Contracts;
using StudyGO.Contracts.Contracts;

namespace StudyGO.Core.Abstractions.VerificationStrategy;

public interface IVerificationStrategy
{
    RegistryScheme Scheme { get; }
    
    Task VerifyAsync(RegisteredInformation @event, CancellationToken cancellationToken);
}