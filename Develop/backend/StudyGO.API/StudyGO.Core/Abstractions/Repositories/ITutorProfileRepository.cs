using StudyGO.Contracts.Result;
using StudyGO.Core.Abstractions.Base.DataCrud;
using StudyGO.Core.Models;

namespace StudyGO.Core.Abstractions.Repositories
{
    public interface ITutorProfileRepository : IReadable<TutorProfile, Guid>
    {
        Task<Result<Guid>> Create(TutorProfile model);
        Task<Result<Guid>> Update(TutorProfile model);
    }
}
