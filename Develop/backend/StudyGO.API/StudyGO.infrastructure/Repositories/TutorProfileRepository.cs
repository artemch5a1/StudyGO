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
    public class TutorProfileRepository : ITutorProfileRepository
    {
        private readonly ApplicationDbContext _context;

        private readonly IMapper _mapper;

        private readonly ILogger<TutorProfileRepository> _logger;

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

        public async Task<Result<Guid>> Create(
            TutorProfile model,
            CancellationToken cancellationToken = default
        )
        {
            _logger.LogInformation("Попытка создания профиля");

            UserEntity user = _mapper.Map<UserEntity>(model.User);

            if (user.Role != RolesEnum.Tutor.GetString())
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

                var profile = _mapper.Map<TutorProfileEntity>(model);

                profile.UserId = userEntry.Entity.UserId;

                profile.User = null;

                profile.Format = null;

                await _context.TutorProfilesEntity.AddAsync(profile, cancellationToken);

                await _context.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);

                _logger.LogInformation("Профиль успешно создан");

                return Result<Guid>.Success(profile.UserId);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);

                _logger.LogError($"Произошла ошибка при создании аккаунта учителя: {ex.Message}");

                return ex.HandleException<Guid>();
            }
        }

        public async Task<Result<List<TutorProfile>>> GetAll(
            CancellationToken cancellationToken = default
        )
        {
            try
            {
                List<TutorProfileEntity> user = await _context
                    .TutorProfilesEntity.Include(x => x.User)
                    .Include(x => x.Format)
                    .Include(x => x.TutorSubjects)
                    .ThenInclude(x => x.Subject)
                    .ToListAsync(cancellationToken);

                return Result<List<TutorProfile>>.Success(_mapper.Map<List<TutorProfile>>(user));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Возникла ошибка при получении данных из БД: {ex.Message}");

                return ex.HandleException<List<TutorProfile>>();
            }
        }

        public async Task<Result<List<TutorProfile>>> GetPages(int skip, int take, CancellationToken cancellationToken = default)
        {
            try
            {
                List<TutorProfileEntity> user = await _context
                    .TutorProfilesEntity.Include(x => x.User)
                    .Include(x => x.Format)
                    .Include(x => x.TutorSubjects)
                    .ThenInclude(x => x.Subject)
                    .Skip(skip)
                    .Take(take)
                    .ToListAsync(cancellationToken);

                return Result<List<TutorProfile>>.Success(_mapper.Map<List<TutorProfile>>(user));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Возникла ошибка при получении данных из БД: {ex.Message}");

                return ex.HandleException<List<TutorProfile>>();
            }
        }
        
        public async Task<Result<TutorProfile?>> GetById(
            Guid id,
            CancellationToken cancellationToken = default
        )
        {
            try
            {
                TutorProfileEntity? user = await _context
                    .TutorProfilesEntity.Include(x => x.User)
                    .Include(x => x.Format)
                    .Include(x => x.TutorSubjects)
                    .ThenInclude(x => x.Subject)
                    .FirstOrDefaultAsync(x => x.UserId == id, cancellationToken);

                if (user == null)
                    return Result<TutorProfile?>.Failure(
                        "Пользователь не найден",
                        ErrorTypeEnum.NotFound
                    );

                return Result<TutorProfile?>.Success(_mapper.Map<TutorProfile?>(user));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Возникла ошибка при получении данных из БД: {ex.Message}");

                return ex.HandleException<TutorProfile?>();
            }
        }

        public async Task<Result<Guid>> Update(
            TutorProfile model,
            CancellationToken cancellationToken = default
        )
        {
            try
            {
                TutorProfileEntity entity = _mapper.Map<TutorProfileEntity>(model);

                var tutorProfile = await _context.TutorProfilesEntity
                    .Include(e => e.TutorSubjects) 
                    .FirstOrDefaultAsync(e => e.UserId == entity.UserId, cancellationToken);

                int affectedRows = 0;
                
                if (tutorProfile != null)
                {
                    tutorProfile.PricePerHour = entity.PricePerHour;
                    tutorProfile.City = entity.City;
                    tutorProfile.FormatId = entity.FormatId;
                    tutorProfile.Bio = entity.Bio;
                    
                    tutorProfile.TutorSubjects.Clear(); 
                    foreach (var subject in entity.TutorSubjects)
                    {
                        tutorProfile.TutorSubjects.Add(subject);
                    }
    
                    affectedRows = await _context.SaveChangesAsync(cancellationToken);
                }
                
                return affectedRows > 0
                    ? Result<Guid>.Success(model.UserId)
                    : Result<Guid>.Failure("Не удалось обновить данные", ErrorTypeEnum.NotFound);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Произошла ошибка при попытке обновить БД: {ex.Message}");

                return ex.HandleException<Guid>();
            }
        }
    }
}
