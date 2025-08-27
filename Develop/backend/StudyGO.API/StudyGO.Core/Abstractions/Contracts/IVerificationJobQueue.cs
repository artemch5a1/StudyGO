using StudyGO.Contracts.Contracts;

namespace StudyGO.Core.Abstractions.Contracts;

public interface IVerificationJobQueue
{
    ValueTask EnqueueAsync(VerificationJob job, CancellationToken cancellationToken);
    IAsyncEnumerable<VerificationJob> DequeueAllAsync(CancellationToken cancellationToken);
}