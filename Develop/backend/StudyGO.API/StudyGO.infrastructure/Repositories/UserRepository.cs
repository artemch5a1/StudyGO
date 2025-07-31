using Microsoft.EntityFrameworkCore;
using StudyGO.Contracts.Contracts;
using StudyGO.Core.Abstractions.Repositories;
using StudyGO.Core.Models;
using StudyGO.infrastructure.Data;

namespace StudyGO.infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Delete(Guid id)
        {
            try
            {
                int count = await _context
                    .UsersEntity.Where(x => x.UserID == id)
                    .ExecuteDeleteAsync();
                if (count != 0)
                {
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<User>> GetAll()
        {
            throw new NotImplementedException();
        }

        public async Task<User> GetById(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<UserLoginResponse> LoginAction(UserLoginRequest login)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> Update(User user)
        {
            throw new NotImplementedException();
        }
    }
}
