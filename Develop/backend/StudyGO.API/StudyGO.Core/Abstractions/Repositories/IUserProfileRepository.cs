using StudyGO.Core.Abstractions.Base.DataCrud;
using StudyGO.Core.Models;

namespace StudyGO.Core.Abstractions.Repositories
{
    public interface IUserProfileRepository : IReadable<UserProfile, Guid>
    {
        Task<Guid> Create(UserProfile model);
        Task<bool> Update(UserProfile model);
    }
}
