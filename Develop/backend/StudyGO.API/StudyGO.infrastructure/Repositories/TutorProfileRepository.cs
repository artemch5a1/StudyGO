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
    public class TutorProfileRepository : ITutorProfileRepository
    {
        private ApplicationDbContext _context;

        private IMapper _mapper;

        private ILogger<TutorProfileRepository> _logger;

        public TutorProfileRepository(
            ApplicationDbContext context,
            IMapper mapper,
            ILogger<TutorProfileRepository> logger
        )
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Guid> Create(TutorProfile model)
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

                var profile = _mapper.Map<TutorProfileEntity>(model);

                profile.UserID = userEntry.Entity.UserID;

                await _context.TutorProfilesEntity.AddAsync(profile);

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

        public async Task<List<TutorProfile>> GetAll()
        {
            try
            {
                List<TutorProfileEntity> user = await _context
                    .TutorProfilesEntity.Include(x => x.User)
                    .Include(x => x.Format)
                    .ToListAsync();
                return _mapper.Map<List<TutorProfile>>(user);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Возникла ошибка при получении данных из БД: {ex.Message}");
                return new();
            }
        }

        public async Task<TutorProfile?> GetById(Guid id)
        {
            try
            {
                TutorProfileEntity? user = await _context
                    .TutorProfilesEntity.Include(x => x.User)
                    .Include(x => x.Format)
                    .FirstOrDefaultAsync(x => x.UserID == id);
                return _mapper.Map<TutorProfile?>(user);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Возникла ошибка при получении данных из БД: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> Update(TutorProfile model)
        {
            try
            {
                TutorProfileEntity entity = _mapper.Map<TutorProfileEntity>(model);
                _context.TutorProfilesEntity.Update(entity);
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
