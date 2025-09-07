using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StudyGO.Core.Abstractions.CleanupServices;
using StudyGO.Core.Abstractions.Repositories;
using StudyGO.infrastructure.Data.CleanUpServices.Options;

namespace StudyGO.infrastructure.Data.CleanUpServices;

public class UserVerificationCleanup : IDatabaseCleanupService
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UserVerificationCleanup> _logger;
    private readonly UserVerificationCleanUpOptions _options;

    public UserVerificationCleanup(
        IUserRepository userRepository,
        ILogger<UserVerificationCleanup> logger,
        IOptions<UserVerificationCleanUpOptions> options)
    {
        _userRepository = userRepository;
        _logger = logger;
        _options = options.Value;
    }
    
    public async Task<int> CleanupAsync(CancellationToken cancellationToken)
    {
        var result = await _userRepository
            .RemoveAllUnverifiedUserByTimeout(TimeSpan.FromMinutes(_options.TimeForVerification), cancellationToken);
        
        _logger.LogInformation("Удалено неподтвержденных пользователей: {Count}", result.Value);
        
        return result.Value;
    }
}