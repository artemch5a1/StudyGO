using StudyGO.Contracts.Result;

namespace StudyGO.Core.Abstractions.DatabaseSeed;

public interface ISeedService
{
    Task<Result<int>> SeedDataAsync();
}