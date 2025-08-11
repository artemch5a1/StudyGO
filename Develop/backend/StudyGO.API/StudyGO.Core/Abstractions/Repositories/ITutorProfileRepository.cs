using StudyGO.Contracts.Result;
using StudyGO.Core.Abstractions.Base.DataCrud;
using StudyGO.Core.Models;

namespace StudyGO.Core.Abstractions.Repositories
{
    public interface ITutorProfileRepository : IReadable<TutorProfile, Guid>
    {
        Task<Result<Guid>> Create(TutorProfile model, CancellationToken cancellationToken = default);
        Task<Result<Guid>> Update(TutorProfile model, CancellationToken cancellationToken = default);
        Task<Result<List<TutorProfile>>> GetPages(int skip, int take, CancellationToken cancellationToken = default);
    }
}
