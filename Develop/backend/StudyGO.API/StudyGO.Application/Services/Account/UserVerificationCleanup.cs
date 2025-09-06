using Microsoft.Extensions.Logging;
using StudyGO.Core.Abstractions.CleanupServices;
using StudyGO.Core.Abstractions.Repositories;

namespace StudyGO.Application.Services.Account;

public class UserVerificationCleanup : IDatabaseCleanupService
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UserVerificationCleanup> _logger;

    public UserVerificationCleanup(IUserRepository userRepository, ILogger<UserVerificationCleanup> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }
    
    public async Task<int> CleanupAsync(CancellationToken cancellationToken)
    {
        var result = await _userRepository.RemoveAllUnverifiedUserByTimeout(TimeSpan.FromMinutes(30), cancellationToken);
        
        _logger.LogInformation("Удалено неподтвержденных пользователей: {Count}", result.Value);
        
        return result.Value;
    }
}