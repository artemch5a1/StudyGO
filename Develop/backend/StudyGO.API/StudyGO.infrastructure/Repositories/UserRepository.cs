using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StudyGO.Contracts.Contracts;
using StudyGO.Core.Abstractions.Repositories;
using StudyGO.Core.Models;
using StudyGO.infrastructure.Data;
using StudyGO.infrastructure.Entites;

namespace StudyGO.infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private ApplicationDbContext _context;

        private IMapper _mapper;

        private ILogger<UserRepository> _logger;

        public UserRepository(
            ApplicationDbContext context,
            IMapper mapper,
            ILogger<UserRepository> logger
        )
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
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
                    _logger.LogInformation($"Пользователь с айди {id} был удален");
                    return true;
                }
                _logger.LogWarning($"Пользователь с айди {id} не был удален");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Произошла ошибка при удалении записи из БД: {ex.Message}");
                return false;
            }
        }

        public async Task<List<User>> GetAll()
        {
            try
            {
                return _mapper.Map<List<User>>(await _context.UsersEntity.ToListAsync());
            }
            catch (Exception ex)
            {
                _logger.LogError($"Произошла ошибка при получении записей из БД: {ex.Message}");
                return new List<User>();
            }
        }

        public async Task<User?> GetById(Guid id)
        {
            try
            {
                return _mapper.Map<User?>(
                    await _context.UsersEntity.FirstOrDefaultAsync(x => x.UserID == id)
                );
            }
            catch (Exception ex)
            {
                _logger.LogError($"Произошла ошибка при получении записи из БД: {ex.Message}");
                return null;
            }
        }

        public async Task<UserLoginResponse> LoginAction(UserLoginRequest login)
        {
            try
            {
                _logger.LogInformation($"Login action: {login}");
                UserEntity? userEntity = await _context.UsersEntity.SingleOrDefaultAsync(x =>
                    x.Email == login.Email
                );

                return new UserLoginResponse
                {
                    IsLoggedIn = userEntity?.PasswordHash == login.PasswordHash,
                    Role = userEntity?.Role ?? string.Empty,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Login action error: {ex.Message}");
                return new UserLoginResponse
                {
                    IsLoggedIn = false,
                    Role = string.Empty,
                };
            }
        }

        public async Task<bool> Update(User user)
        {
            try
            {
                _context.UsersEntity.Update(_mapper.Map<UserEntity>(user));
                int affectedRows = await _context.SaveChangesAsync();

                return affectedRows > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Произошла ошибка при попытке обновить БД: {ex.Message}");
                return false;
            }
        }
    }
}
