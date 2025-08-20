using StudyGO.Contracts.Contracts;
using StudyGO.Contracts.Result;
using StudyGO.Core.Abstractions.Base.DataCrud;
using StudyGO.Core.Models;

namespace StudyGO.Core.Abstractions.Repositories
{
    public interface IUserRepository : IReadable<User, Guid>
    {
        public Task<Result<Guid>> UpdateСredentials(User user, CancellationToken cancellationToken = default);

        public Task<Result<Guid>> Update(User user, CancellationToken cancellationToken = default);

        public Task<Result<Guid>> Delete(Guid id, CancellationToken cancellationToken = default);

        public Task<Result<UserLoginResponse>> GetCredentialByEmail(string email, CancellationToken cancellationToken = default);

        public Task<Result<Guid>> ConfirmEmailAsync(Guid userId, string userToken, CancellationToken cancellationToken = default);

        public Task<Result<Guid>> InsertVerifiedToken(Guid userId, string verifiedToken,
            CancellationToken cancellationToken = default);

        public Task<Result<int>> RemoveAllUnverifiedUserByTimeout(
            TimeSpan timeout,
            CancellationToken cancellationToken = default
        );
    }
}
