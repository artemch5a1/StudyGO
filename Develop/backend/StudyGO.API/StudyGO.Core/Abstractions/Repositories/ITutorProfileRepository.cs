using StudyGO.Core.Abstractions.Base.DataCrud;
using StudyGO.Core.Models;

namespace StudyGO.Core.Abstractions.Repositories
{
    public interface ITutorProfileRepository : IReadable<TutorProfile, Guid>
    {
        Task<bool> Create(TutorProfile model);
        Task<bool> Update(TutorProfile model);
    }
}
