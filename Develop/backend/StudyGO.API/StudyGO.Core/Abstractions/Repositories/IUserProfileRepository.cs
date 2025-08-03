using StudyGO.Contracts.Result;
using StudyGO.Core.Abstractions.Base.DataCrud;
using StudyGO.Core.Models;

namespace StudyGO.Core.Abstractions.Repositories
{
    public interface IUserProfileRepository : IReadable<UserProfile, Guid>
    {
        Task<Result<Guid>> Create(UserProfile model);
        Task<Result<Guid>> Update(UserProfile model);
    }
}
