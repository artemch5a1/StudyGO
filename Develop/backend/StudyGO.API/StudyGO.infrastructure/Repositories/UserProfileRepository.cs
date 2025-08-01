using System.Data;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StudyGO.Core.Abstractions.Repositories;
using StudyGO.Core.Enums;
using StudyGO.Core.Extensions;
using StudyGO.Core.Models;
using StudyGO.infrastructure.Data;
using StudyGO.infrastructure.Entites;

namespace StudyGO.infrastructure.Repositories
{
    public class UserProfileRepository : IUserProfileRepository
    {
        private ApplicationDbContext _context;

        private IMapper _mapper;

        private ILogger<UserRepository> _logger;

        public UserProfileRepository(
            ApplicationDbContext context,
            IMapper mapper,
            ILogger<UserRepository> logger
        )
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Guid> Create(UserProfile model)
        {
            _logger.LogInformation("Попытка создания профиля");

            UserEntity user = _mapper.Map<UserEntity>(model.User);

            if (user.Role != RolesEnum.user.GetString())
            {
                _logger.LogError("Неверная роль, откат операции");
                return Guid.Empty;
            }

            await using var transaction = await _context.Database.BeginTransactionAsync(
                isolationLevel: IsolationLevel.ReadUncommitted
            );

            try
            {
                var userEntry = await _context.UsersEntity.AddAsync(user);

                await _context.SaveChangesAsync();

                var profile = _mapper.Map<UserProfileEntity>(model);

                profile.UserID = userEntry.Entity.UserID;

                await _context.UserProfilesEntity.AddAsync(profile);

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                _logger.LogInformation("Профиль успешно создан");
                return profile.UserID;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError($"Произошла ошибка при создании аккаунта учителя: {ex.Message}");
                return Guid.Empty;
            }
        }

        public async Task<List<UserProfile>> GetAll()
        {
            try
            {
                List<UserProfileEntity> user = await _context.UserProfilesEntity.ToListAsync();
                return _mapper.Map<List<UserProfile>>(user);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Возникла ошибка при получении данных из БД: {ex.Message}");
                return new();
            }
        }

        public async Task<UserProfile?> GetById(Guid id)
        {
            try
            {
                UserProfileEntity? user = await _context.UserProfilesEntity.FirstOrDefaultAsync(x =>
                    x.UserID == id
                );
                return _mapper.Map<UserProfile?>(user);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Возникла ошибка при получении данных из БД: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> Update(UserProfile model)
        {
            try
            {
                UserProfileEntity entity = _mapper.Map<UserProfileEntity>(model);
                _context.UserProfilesEntity.Update(entity);
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
