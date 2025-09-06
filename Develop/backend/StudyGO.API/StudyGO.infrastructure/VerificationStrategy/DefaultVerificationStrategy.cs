using StudyGO.Contracts;
using StudyGO.Contracts.Contracts;
using StudyGO.Core.Abstractions.Repositories;
using StudyGO.Core.Abstractions.VerificationStrategy;

namespace StudyGO.infrastructure.VerificationStrategy;

public class DefaultVerificationStrategy : IVerificationStrategy
{
    private readonly IUserRepository _userRepository;

    public DefaultVerificationStrategy(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public RegistryScheme Scheme => RegistryScheme.DefaultVerified;
    
    public async Task VerifyAsync(RegisteredInformation @event, CancellationToken cancellationToken)
    {
        var result = await _userRepository.DefaultVerification(@event.UserId, cancellationToken);
    }
}