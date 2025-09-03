using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StudyGO.Contracts.Contracts;
using StudyGO.Contracts.Result;
using StudyGO.Contracts.Result.ErrorTypes;
using StudyGO.Core.Abstractions.Repositories;
using StudyGO.Core.Models;
using StudyGO.infrastructure.Data;
using StudyGO.infrastructure.Entities;
using StudyGO.infrastructure.Extensions;

namespace StudyGO.infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        private readonly IMapper _mapper;

        private readonly ILogger<UserRepository> _logger;

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

        public async Task<Result<Guid>> Delete(
            Guid id,
            CancellationToken cancellationToken = default
        )
        {
            try
            {
                int count = await _context
                    .UsersEntity.Where(x => x.UserId == id)
                    .ExecuteDeleteAsync(cancellationToken);
                if (count != 0)
                {
                    _logger.LogInformation($"Пользователь с айди {id} был удален");

                    return Result<Guid>.Success(id);
                }
                _logger.LogWarning($"Пользователь с айди {id} не был удален");

                return Result<Guid>.Failure("Ошибка удаления", ErrorTypeEnum.NotFound);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Произошла ошибка при удалении записи из БД: {ex.Message}");

                return ex.HandleException<Guid>();
            }
        }

        public async Task<Result<List<User>>> GetAll(CancellationToken cancellationToken = default)
        {
            try
            {
                List<User> users = _mapper.Map<List<User>>(
                    await _context.UsersEntity.ToListAsync(cancellationToken)
                );

                return Result<List<User>>.Success(users);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Произошла ошибка при получении записей из БД: {ex.Message}");

                return ex.HandleException<List<User>>();
            }
        }

        public async Task<Result<User?>> GetById(
            Guid id,
            CancellationToken cancellationToken = default
        )
        {
            try
            {
                User? user = _mapper.Map<User?>(
                    await _context.UsersEntity.FirstOrDefaultAsync(
                        x => x.UserId == id,
                        cancellationToken
                    )
                );

                if (user != null)
                    return Result<User?>.Success(user);

                return Result<User?>.Failure("Пользователь не найден", ErrorTypeEnum.NotFound);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Произошла ошибка при получении записи из БД: {ex.Message}");

                return ex.HandleException<User?>();
            }
        }

        public async Task<Result<UserLoginResponse>> GetCredentialByEmail(
            string email,
            CancellationToken cancellationToken = default
        )
        {
            try
            {
                UserLoginResponse response =
                    await _context
                        .UsersEntity.Where(u => u.Email == email && u.Verified)
                        .Select(u => new UserLoginResponse
                        {
                            Email = u.Email,
                            PasswordHash = u.PasswordHash,
                            Role = u.Role,
                            Id = u.UserId,
                        })
                        .FirstOrDefaultAsync(cancellationToken) ?? new UserLoginResponse();

                return Result<UserLoginResponse>.Success(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    $"Произошла ошибка при получении учетных данных из БД: {ex.Message}"
                );

                return ex.HandleException<UserLoginResponse>();
            }
        }

        public async Task<Result<Guid>> UpdatePassword(
            Guid userId, string newPasswordHash,
            CancellationToken cancellationToken = default
        )
        {
            try
            {
                int result = await _context
                    .UsersEntity.Where(e => e.UserId == userId)
                    .ExecuteUpdateAsync(
                        s =>
                            s.SetProperty(i => i.PasswordHash, i => newPasswordHash),
                        cancellationToken
                    );

                if (result < 1)
                    return Result<Guid>.Failure("Данные не были обновлены", ErrorTypeEnum.NotFound);

                return Result<Guid>.Success(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Произошла ошибка при попытке обновить БД: {ex.Message}");

                return ex.HandleException<Guid>();
            }
        }

        public async Task<Result<Guid>> Update(
            User user,
            CancellationToken cancellationToken = default
        )
        {
            try
            {
                UserEntity entity = _mapper.Map<UserEntity>(user);

                int result = await _context
                    .UsersEntity.Where(e => e.UserId == entity.UserId)
                    .ExecuteUpdateAsync(
                        s =>
                            s.SetProperty(i => i.Surname, i => user.Surname)
                                .SetProperty(i => i.Number, i => user.Number)
                                .SetProperty(i => i.Name, i => user.Name)
                                .SetProperty(i => i.Patronymic, i => user.Patronymic),
                        cancellationToken
                    );

                if (result < 1)
                    return Result<Guid>.Failure("Данные не были обновлены", ErrorTypeEnum.NotFound);

                return Result<Guid>.Success(user.UserId);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Произошла ошибка при попытке обновить БД: {ex.Message}");

                return ex.HandleException<Guid>();
            }
        }

        public async Task<Result<Guid>> InsertVerifiedToken(Guid userId, string verifiedToken, CancellationToken cancellationToken = default)
        {
            try
            {
                int result = await _context.UsersEntity.Where(e => e.UserId == userId).ExecuteUpdateAsync(
                    s =>
                        s.SetProperty(i => i.VerifiedToken, i => verifiedToken),
                    cancellationToken
                );

                if (result > 0)
                    return Result<Guid>.Success(userId);

                return Result<Guid>.Failure("Пользователь не найден", ErrorTypeEnum.NotFound);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Произошла ошибка при попытке обновить БД: {ex.Message}");

                return ex.HandleException<Guid>();
            }
        }

        public async Task<Result<Guid>> ConfirmEmailAsync(Guid userId, string userToken, CancellationToken cancellationToken = default)
        {
            var result = await _context.UsersEntity
                .FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);

            if (result == null || result.Verified)
                return Result<Guid>.Failure("Неактуальный запрос");

            if(result.VerifiedToken == userToken)
            {
                try
                {
                    result.Verified = true;
                    result.VerifiedToken = null;
                    result.VerifiedDate = DateTime.UtcNow;
                    int affectedRows = await _context.SaveChangesAsync(cancellationToken);

                    if(affectedRows > 0)
                    {
                        return Result<Guid>.Success(result.UserId);
                    }

                    return Result<Guid>.Failure("Ошибка обновления данных", ErrorTypeEnum.DbError);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Произошла ошибка при попытке обновить БД: {ex.Message}");

                    return ex.HandleException<Guid>();
                }
            }
            
            return Result<Guid>.Failure("Верификация email провалена", ErrorTypeEnum.ValidationError);
        }

        public async Task<Result<int>> RemoveAllUnverifiedUserByTimeout(
            TimeSpan timeout, 
            CancellationToken cancellationToken = default
            )
        {
            try
            {
                var result = await _context.UsersEntity
                    .Where(x => !x.Verified &&  DateTime.UtcNow - x.DateRegistry > timeout)
                    .ExecuteDeleteAsync(cancellationToken);

                return Result<int>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Произошла ошибка при попытке обновить БД: {ex.Message}");

                return ex.HandleException<int>();
            }
        }

        public async Task<Result<Guid>> DefaultVerification(Guid userId, CancellationToken cancellationToken = default)
        {
            try
            {
                var user = 
                    await _context.UsersEntity
                        .FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);
                
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
