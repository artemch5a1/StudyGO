using System.Data;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StudyGO.Contracts.Result;
using StudyGO.Core.Abstractions.Repositories;
using StudyGO.Core.Enums;
using StudyGO.Core.Extensions;
using StudyGO.Core.Models;
using StudyGO.infrastructure.Data;
using StudyGO.infrastructure.Entites;
using StudyGO.infrastructure.ExceptionHandlers;

namespace StudyGO.infrastructure.Repositories
{
    public class UserProfileRepository : IUserProfileRepository
    {
        private readonly ApplicationDbContext _context;

        private readonly IMapper _mapper;

        private readonly ILogger<UserProfileRepository> _logger;

        public UserProfileRepository(
            ApplicationDbContext context,
            IMapper mapper,
            ILogger<UserProfileRepository> logger
        )
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<Guid>> Create(UserProfile model)
        {
            _logger.LogInformation("Попытка создания профиля");

            UserEntity user = _mapper.Map<UserEntity>(model.User);

            if (user.Role != RolesEnum.user.GetString())
            {
                _logger.LogError("Неверная роль, откат операции");
                return Result<Guid>.Failure("Неверная роль");
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

                profile.User = null;

                profile.FavoriteSubject = null;

                await _context.UserProfilesEntity.AddAsync(profile);

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                _logger.LogInformation("Профиль успешно создан");

                return Result<Guid>.Success(userEntry.Entity.UserID);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                _logger.LogError($"Произошла ошибка при создании аккаунта учителя: {ex.Message}");

                return DatabaseExceptionHandler.HandleException<Guid>(ex);
            }
        }

        public async Task<Result<List<UserProfile>>> GetAll()
        {
            try
            {
                List<UserProfileEntity> user = await _context
                    .UserProfilesEntity.Include(x => x.User)
                    .Include(x => x.FavoriteSubject)
                    .ToListAsync();

                return Result<List<UserProfile>>.Success(_mapper.Map<List<UserProfile>>(user));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Возникла ошибка при получении данных из БД: {ex.Message}");

                return DatabaseExceptionHandler.HandleException<List<UserProfile>>(ex);
            }
        }

        public async Task<Result<UserProfile?>> GetById(Guid id)
        {
            try
            {
                UserProfileEntity? user = await _context
                    .UserProfilesEntity.Include(x => x.User)
                    .Include(x => x.FavoriteSubject)
                    .FirstOrDefaultAsync(x => x.UserID == id);

                if (user == null)
                    return Result<UserProfile?>.Failure("Пользователь не найден");

                return Result<UserProfile?>.Success(_mapper.Map<UserProfile?>(user));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Возникла ошибка при получении данных из БД: {ex.Message}");

                return DatabaseExceptionHandler.HandleException<UserProfile?>(ex);
            }
        }

        public async Task<Result<Guid>> Update(UserProfile model)
        {
            try
            {
                UserProfileEntity entity = _mapper.Map<UserProfileEntity>(model);

                await _context
                    .UserProfilesEntity.Where(e => e.UserID == model.UserID)
                    .ExecuteUpdateAsync(u =>
                        u.SetProperty(i => i.SubjectID, i => model.SubjectID)
                            .SetProperty(i => i.DateBirth, i => model.DateBirth)
                            .SetProperty(i => i.Description, i => model.Description)
                    );

                int affectedRows = await _context.SaveChangesAsync();

                return affectedRows > 0
                    ? Result<Guid>.Success(model.UserID)
                    : Result<Guid>.Failure("Строка не была обновлена");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Произошла ошибка при попытке обновить БД: {ex.Message}");

                return DatabaseExceptionHandler.HandleException<Guid>(ex);
            }
        }
    }
}
