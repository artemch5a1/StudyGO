using StudyGO.Contracts.Result;

namespace StudyGO.Core.Abstractions.Verification;

public interface IVerificationService
{
    Task<Result<string>> CreateTokenAndSendMessage(Guid userId, string email, string endPoint, CancellationToken cancellationToken);

    Task RollBackUser(Guid userId, CancellationToken cancellationToken = default);
}