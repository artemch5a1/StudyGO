using StudyGO.Contracts.Result;
using StudyGO.Core.Abstractions.Base.DataCrud;
using StudyGO.Core.Models;

namespace StudyGO.Core.Abstractions.Repositories
{
    public interface IUserProfileRepository : IReadable<UserProfile, Guid>
    {
        Task<Result<Guid>> Create(UserProfile model, CancellationToken cancellationToken = default);
        Task<Result<Guid>> Update(UserProfile model, CancellationToken cancellationToken = default);
        Task<Result<List<UserProfile>>> GetPages(int skip, int take, CancellationToken cancellationToken = default);
        Task<Result<Guid>> DefaultVerification(Guid userId, CancellationToken cancellationToken = default);
        
        Task<Result<List<UserProfile>>> GetAllVerified(
            CancellationToken cancellationToken = default
        );

        Task<Result<List<UserProfile>>> GetPagesVerified(int skip, int take,
            CancellationToken cancellationToken = default);

        Task<Result<UserProfile?>> GetByIdVerified(
            Guid id,
            CancellationToken cancellationToken = default
        );
    }
}
