using StudyGO.Core.Models;

namespace StudyGO.Core.Abstractions.Repositories
{
    internal interface IUserRepository
    {
        public List<User> GetAll();

        public User GetById(int id);

        public bool Update(User user);

        public bool Delete(int id);
    }
}
