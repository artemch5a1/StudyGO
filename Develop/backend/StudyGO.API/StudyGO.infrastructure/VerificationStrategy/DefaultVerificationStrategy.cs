using StudyGO.Contracts;
using StudyGO.Contracts.Contracts;
using StudyGO.Core.Abstractions.VerificationStrategy;
using StudyGO.infrastructure.Extensions;
using StudyGO.infrastructure.Repositories;

namespace StudyGO.infrastructure.VerificationStrategy;

public class DefaultVerificationStrategy : IVerificationStrategy
{
    private readonly UserRepository _userRepository;

    public DefaultVerificationStrategy(UserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public RegistryScheme Scheme => RegistryScheme.DefaultVerified;
    
    public async Task VerifyAsync(RegisteredInformation @event, CancellationToken cancellationToken)
    {
        var result = await _userRepository.DefaultVerification(@event.UserId, cancellationToken);
    }
}