using System.Data;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StudyGO.Contracts.Result;
using StudyGO.Contracts.Result.ErrorTypes;
using StudyGO.Core.Abstractions.Repositories;
using StudyGO.Core.Enums;
using StudyGO.Core.Extensions;
using StudyGO.Core.Models;
using StudyGO.infrastructure.Data;
using StudyGO.infrastructure.Entities;
using StudyGO.infrastructure.Extensions;

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

        public async Task<Result<Guid>> Create(
            UserProfile model,
            CancellationToken cancellationToken = default
        )
        {
            _logger.LogInformation("Попытка создания профиля");

            UserEntity user = _mapper.Map<UserEntity>(model.User);

            if (user.Role != RolesEnum.User.GetString())
            {
                _logger.LogError("Неверная роль, откат операции");
                return Result<Guid>.Failure("Неверная роль", ErrorTypeEnum.ServerError);
            }

            bool isExistEmail = await _context.UsersEntity.AnyAsync(
                x => x.Email == user.Email,
                cancellationToken
            );

            if (isExistEmail)
                return Result<Guid>.Failure(
                    $"Пользователь с таким email уже существует",
                    ErrorTypeEnum.Duplicate
                );

            await using var transaction = await _context.Database.BeginTransactionAsync(
                isolationLevel: IsolationLevel.ReadUncommitted,
                cancellationToken
            );

            try
            {
                var userEntry = await _context.UsersEntity.AddAsync(user, cancellationToken);

                await _context.SaveChangesAsync(cancellationToken);

                var profile = _mapper.Map<UserProfileEntity>(model);

                profile.UserId = userEntry.Entity.UserId;

                profile.User = null;

                profile.FavoriteSubject = null;

                await _context.UserProfilesEntity.AddAsync(profile, cancellationToken);

                await _context.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);

                _logger.LogInformation("Профиль успешно создан");

                return Result<Guid>.Success(userEntry.Entity.UserId);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);

                _logger.LogError($"Произошла ошибка при создании аккаунта учителя: {ex.Message}");

                return ex.HandleException<Guid>();
            }
        }

        public async Task<Result<List<UserProfile>>> GetAll(
            CancellationToken cancellationToken = default
        )
        {
            try
            {
                List<UserProfileEntity> user = await _context
                    .UserProfilesEntity.Include(x => x.User)
                    .Include(x => x.FavoriteSubject)
                    .ToListAsync(cancellationToken);

                return Result<List<UserProfile>>.Success(_mapper.Map<List<UserProfile>>(user));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Возникла ошибка при получении данных из БД: {ex.Message}");

                return ex.HandleException<List<UserProfile>>();
            }
        }

        public async Task<Result<List<UserProfile>>> GetAllVerified(
            CancellationToken cancellationToken = default
        )
        {
            try
            {
                List<UserProfileEntity> user = await _context
                    .UserProfilesEntity.Include(x => x.User)
                    .Include(x => x.FavoriteSubject)
                    .Where(x => x.User != null && x.User.Verified)
                    .ToListAsync(cancellationToken);

                return Result<List<UserProfile>>.Success(_mapper.Map<List<UserProfile>>(user));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Возникла ошибка при получении данных из БД: {ex.Message}");

                return ex.HandleException<List<UserProfile>>();
            }
        }

        public async Task<Result<List<UserProfile>>> GetPages(int skip, int take, CancellationToken cancellationToken = default)
        {
            try
            {
                List<UserProfileEntity> user = await _context
                    .UserProfilesEntity.Include(x => x.User)
                    .Include(x => x.FavoriteSubject)
                    .OrderBy(x => x.UserId)
                    .Skip(skip)
                    .Take(take)
                    .ToListAsync(cancellationToken);

                return Result<List<UserProfile>>.Success(_mapper.Map<List<UserProfile>>(user));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Возникла ошибка при получении данных из БД: {ex.Message}");

                return ex.HandleException<List<UserProfile>>();
            }
        }

        public async Task<Result<List<UserProfile>>> GetPagesVerified(int skip, int take,
            CancellationToken cancellationToken = default)
        {
            try
            {
                List<UserProfileEntity> user = await _context
                    .UserProfilesEntity.Include(x => x.User)
                    .Include(x => x.FavoriteSubject)
                    .OrderBy(x => x.UserId)
                    .Skip(skip)
                    .Take(take)
                    .Where(x => x.User != null && x.User.Verified)
                    .ToListAsync(cancellationToken);

                return Result<List<UserProfile>>.Success(_mapper.Map<List<UserProfile>>(user));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Возникла ошибка при получении данных из БД: {ex.Message}");

                return ex.HandleException<List<UserProfile>>();
            }
        }

        public async Task<Result<UserProfile?>> GetById(
            Guid id,
            CancellationToken cancellationToken = default
        )
        {
            try
            {
                UserProfileEntity? user = await _context
                    .UserProfilesEntity.Include(x => x.User)
                    .Include(x => x.FavoriteSubject)
                    .FirstOrDefaultAsync(x => x.UserId == id, cancellationToken);

                if (user == null)
                    return Result<UserProfile?>.Failure(
                        "Пользователь не найден",
                        ErrorTypeEnum.NotFound
                    );

                return Result<UserProfile?>.Success(_mapper.Map<UserProfile?>(user));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Возникла ошибка при получении данных из БД: {ex.Message}");

                return ex.HandleException<UserProfile?>();
            }
        }

        public async Task<Result<UserProfile?>> GetByIdVerified(
            Guid id,
            CancellationToken cancellationToken = default
        )
        {
            try
            {
                UserProfileEntity? user = await _context
                    .UserProfilesEntity.Include(x => x.User)
                    .Include(x => x.FavoriteSubject)
                    .FirstOrDefaultAsync(x => x.UserId == id && x.User != null && x.User.Verified, cancellationToken);

                if (user == null)
                    return Result<UserProfile?>.Failure(
                        "Пользователь не найден",
                        ErrorTypeEnum.NotFound
                    );

                return Result<UserProfile?>.Success(_mapper.Map<UserProfile?>(user));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Возникла ошибка при получении данных из БД: {ex.Message}");

                return ex.HandleException<UserProfile?>();
            }
        }

        public async Task<Result<Guid>> Update(
            UserProfile model,
            CancellationToken cancellationToken = default
        )
        {
            try
            {
                UserProfileEntity entity = _mapper.Map<UserProfileEntity>(model);

                int affectedRows = await _context
                    .UserProfilesEntity.Where(e => e.UserId == model.UserId)
                    .ExecuteUpdateAsync(
                        u =>
                            u.SetProperty(i => i.SubjectId, i => model.SubjectId)
                                .SetProperty(i => i.DateBirth, i => model.DateBirth)
                                .SetProperty(i => i.Description, i => model.Description),
                        cancellationToken
                    );

                return affectedRows > 0
                    ? Result<Guid>.Success(model.UserId)
                    : Result<Guid>.Failure("Строка не была обновлена", ErrorTypeEnum.NotFound);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Произошла ошибка при попытке обновить БД: {ex.Message}");

                return ex.HandleException<Guid>();
            }
        }

        public async Task<Result<Guid>> DefaultVerification(
            Guid userId, 
            CancellationToken cancellationToken = default
            )
        {
            try
            {
                var user = await _context.UserProfilesEntity
                    .Select(x => x.User)
                    .FirstOrDefaultAsync(x => x != null && x.UserId == userId, cancellationToken);

                if(user == null)
                {
                    _logger.LogError($"Произошла ошибка при попытке подтверждения пользователя: пользователь не найден");
                    return Result<Guid>.Failure("Ошибка сервера", ErrorTypeEnum.DbError);
                }

                user.Verified = true;
                user.VerifiedToken = null;
                user.VerifiedDate = DateTime.UtcNow;

                int affectedRows = await _context.SaveChangesAsync(cancellationToken);

                if(affectedRows > 0)
                    return Result<Guid>.Success(userId);
                
                return Result<Guid>.Failure("Ошибка сервера", ErrorTypeEnum.DbError);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Произошла ошибка при попытке обновить БД: {ex.Message}");

                return ex.HandleException<Guid>();
            }
        }
    }
}
