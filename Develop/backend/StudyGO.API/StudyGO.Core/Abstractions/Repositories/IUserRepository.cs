using StudyGO.Contracts.Contracts;
using StudyGO.Core.Abstractions.Base.DataCrud;
using StudyGO.Core.Models;

namespace StudyGO.Core.Abstractions.Repositories
{
    public interface IUserRepository : IReadable<User>
    {
        public bool Update(User user);

        public bool Delete(int id);

        public UserLoginResponse LoginAction(UserLoginRequest login);
    }
}
