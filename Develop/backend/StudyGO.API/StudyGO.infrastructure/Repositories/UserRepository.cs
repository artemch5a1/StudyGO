using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StudyGO.Contracts.Contracts;
using StudyGO.Contracts.Result;
using StudyGO.Contracts.Result.ErrorTypes;
using StudyGO.Core.Abstractions.Repositories;
using StudyGO.Core.Models;
using StudyGO.infrastructure.Data;
using StudyGO.infrastructure.Entites;
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
                    .UsersEntity.Where(x => x.UserID == id)
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
                        x => x.UserID == id,
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
                        .UsersEntity.Where(u => u.Email == email)
                        .Select(u => new UserLoginResponse
                        {
                            Email = u.Email,
                            PasswordHash = u.PasswordHash,
                            Role = u.Role,
                            Id = u.UserID,
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

        public async Task<Result<Guid>> UpdateСredentials(
            User user,
            CancellationToken cancellationToken = default
        )
        {
            try
            {
                bool isExistEmail = await _context.UsersEntity.AnyAsync(
                    x => x.Email == user.Email,
                    cancellationToken
                );

                if (isExistEmail)
                    return Result<Guid>.Failure(
                        $"Пользователь с таким email уже существует",
                        ErrorTypeEnum.Duplicate
                    );

                UserEntity entity = _mapper.Map<UserEntity>(user);

                int result = await _context
                    .UsersEntity.Where(e => e.UserID == entity.UserID)
                    .ExecuteUpdateAsync(
                        s =>
                            s.SetProperty(i => i.PasswordHash, i => user.PasswordHash)
                                .SetProperty(i => i.Email, i => user.Email),
                        cancellationToken
                    );

                if (result < 1)
                    return Result<Guid>.Failure("Данные не были обновлены", ErrorTypeEnum.NotFound);

                return Result<Guid>.Success(user.UserID);
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
                    .UsersEntity.Where(e => e.UserID == entity.UserID)
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

                return Result<Guid>.Success(user.UserID);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Произошла ошибка при попытке обновить БД: {ex.Message}");

                return ex.HandleException<Guid>();
            }
        }
    }
}
