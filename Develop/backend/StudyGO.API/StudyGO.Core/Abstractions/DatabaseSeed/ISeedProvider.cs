using StudyGO.Contracts.Result;

namespace StudyGO.Core.Abstractions.DatabaseSeed;

public interface ISeedProvider
{
    Task<Result<int>> SeedDataBaseAsync();
}