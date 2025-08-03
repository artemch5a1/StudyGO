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

        public async Task<Result<Guid>> Create(TutorProfile model)
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

                var profile = _mapper.Map<TutorProfileEntity>(model);

                profile.UserID = userEntry.Entity.UserID;

                profile.User = null;

                profile.Format = null;

                await _context.TutorProfilesEntity.AddAsync(profile);

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                _logger.LogInformation("Профиль успешно создан");

                return Result<Guid>.Success(profile.UserID);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                _logger.LogError($"Произошла ошибка при создании аккаунта учителя: {ex.Message}");

                return ex.HandleException<Guid>();
            }
        }

        public async Task<Result<List<TutorProfile>>> GetAll()
        {
            try
            {
                List<TutorProfileEntity> user = await _context
                    .TutorProfilesEntity.Include(x => x.User)
                    .Include(x => x.Format)
                    .ToListAsync();

                return Result<List<TutorProfile>>.Success(_mapper.Map<List<TutorProfile>>(user));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Возникла ошибка при получении данных из БД: {ex.Message}");

                return ex.HandleException<List<TutorProfile>>();
            }
        }

        public async Task<Result<TutorProfile?>> GetById(Guid id)
        {
            try
            {
                TutorProfileEntity? user = await _context
                    .TutorProfilesEntity.Include(x => x.User)
                    .Include(x => x.Format)
                    .FirstOrDefaultAsync(x => x.UserID == id);

                return Result<TutorProfile?>.Success(_mapper.Map<TutorProfile?>(user));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Возникла ошибка при получении данных из БД: {ex.Message}");

                return ex.HandleException<TutorProfile?>();
            }
        }

        public async Task<Result<Guid>> Update(TutorProfile model)
        {
            try
            {
                TutorProfileEntity entity = _mapper.Map<TutorProfileEntity>(model);

                await _context
                    .TutorProfilesEntity.Where(e => e.UserID == model.UserID)
                    .ExecuteUpdateAsync(u =>
                        u.SetProperty(i => i.PricePerHour, i => model.PricePerHour)
                            .SetProperty(i => i.City, i => model.City)
                            .SetProperty(i => i.FormatID, i => model.FormatID)
                            .SetProperty(i => i.Bio, i => model.Bio)
                    );

                int affectedRows = await _context.SaveChangesAsync();

                return affectedRows > 0
                    ? Result<Guid>.Success(model.UserID)
                    : Result<Guid>.Failure("Не удалось обновить данные");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Произошла ошибка при попытке обновить БД: {ex.Message}");

                return ex.HandleException<Guid>();
            }
        }
    }
}
