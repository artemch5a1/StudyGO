namespace StudyGO.Core.Abstractions.CleanupServices;

public interface IDatabaseCleanupService
{
    Task<int> CleanupAsync(CancellationToken cancellationToken);
}