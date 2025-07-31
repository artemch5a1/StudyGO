using StudyGO.Contracts.Contracts;
using StudyGO.Core.Abstractions.Base.DataCrud;
using StudyGO.Core.Models;

namespace StudyGO.Core.Abstractions.Repositories
{
    public interface IUserRepository : IReadable<User, Guid>
    {
        public Task<bool> Update(User user);

        public Task<bool> Delete(int id);

        public Task<UserLoginResponse> LoginAction(UserLoginRequest login);
    }
}
