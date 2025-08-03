using StudyGO.Contracts.Contracts;
using StudyGO.Contracts.Result;
using StudyGO.Core.Abstractions.Base.DataCrud;
using StudyGO.Core.Models;

namespace StudyGO.Core.Abstractions.Repositories
{
    public interface IUserRepository : IReadable<User, Guid>
    {
        public Task<Result<Guid>> UpdateСredentials(User user);

        public Task<Result<Guid>> Update(User user);

        public Task<Result<Guid>> Delete(Guid id);

        public Task<Result<UserLoginResponse>> GetCredentialByEmail(string email);
    }
}
