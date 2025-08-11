using AutoMapper;
using Microsoft.Extensions.Logging;
using StudyGO.Application.Extensions;
using StudyGO.Contracts.Contracts;
using StudyGO.Contracts.Dtos.Users;
using StudyGO.Contracts.Result;
using StudyGO.Contracts.Result.ErrorTypes;
using StudyGO.Core.Abstractions.Auth;
using StudyGO.Core.Abstractions.Repositories;
using StudyGO.Core.Abstractions.Services.Account;
using StudyGO.Core.Abstractions.Utils;
using StudyGO.Core.Abstractions.ValidationService;
using StudyGO.Core.Extensions;
using StudyGO.Core.Models;

namespace StudyGO.Application.Services.Account
{
    public class UserAccountService : IUserAccountService
    {
        private readonly IUserRepository _userRepository;

        private readonly IMapper _mapper;

        private readonly ILogger<UserAccountService> _logger;

        private readonly IPasswordHasher _passwordHasher;

        private readonly IJwtTokenProvider _jwtTokenProvider;

        private readonly IValidationService _validationService;

        public UserAccountService(
            IUserRepository userRepository,
            IMapper mapper,
            ILogger<UserAccountService> logger,
            IPasswordHasher passwordHasher,
            IJwtTokenProvider jwtTokenProvider,
            IValidationService validationService
        )
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _logger = logger;
            _passwordHasher = passwordHasher;
            _jwtTokenProvider = jwtTokenProvider;
            _validationService = validationService;
        }

        public async Task<Result<Guid>> TryDeleteAccount(
            Guid id,
            CancellationToken cancellationToken = default
        )
        {
            _logger.LogDebug("Отправлен запрос на удаление пользователя {UserId}", id);
            return await _userRepository.Delete(id, cancellationToken);
        }

        public async Task<Result<UserDto?>> TryGetAccountById(
            Guid id,
            CancellationToken cancellationToken = default
        )
        {
            _logger.LogInformation("Попытка получить аккаунт по id {UserId}", id);
            Result<User?> result = await _userRepository.GetById(id, cancellationToken);
            
            _logger.LogResult(result, 
                "Успешно найден пользователь",
                "Пользователь не найден",
            new {UserId = id});
            
            return result.MapDataTo(x => _mapper.Map<UserDto?>(x));
        }

        public async Task<Result<List<UserDto>>> TryGetAllAccount(
            CancellationToken cancellationToken = default
        )
        {
            _logger.LogInformation("Попытка получить всех пользователей");
            
            Result<List<User>> result = await _userRepository.GetAll(cancellationToken);
            
            _logger.LogInformation("Получено {count} аккаунтов", result.Value?.Count);
            
            return result.MapDataTo(_mapper.Map<List<UserDto>>);
        }

        public async Task<Result<UserLoginResponseDto>> TryLogIn(
            UserLoginRequest userLogin,
            CancellationToken cancellationToken = default
        )
        {
            _logger.LogInformation("Получение аккаунта по email: {Email}", LoggingExtensions.MaskEmail(userLogin.Email));
            Result<UserLoginResponse> result = await _userRepository.GetCredentialByEmail(
                userLogin.Email,
                cancellationToken
            );
            
            if (!result.IsSuccess)
                return Result<UserLoginResponseDto>.Failure(result.ErrorMessage!, result.ErrorType);

            var dbSearchCred = result.Value ?? new();

            if (result.Value != null)
            {
                _logger.LogInformation("Аккаунт был получен {UserId}", result.Value.Id);
            }
            else
            {
                _logger.LogInformation("Аккаунта не существует");
            }
            
            bool isAccess = IsSuccessUserLogin(userLogin, dbSearchCred);

            if (isAccess)
            {
                _logger.LogInformation("Аутентификация пройдена, генерация токена...");
                
                var responseDto = new UserLoginResponseDto()
                {
                    Token = _jwtTokenProvider.GenerateToken(dbSearchCred),
                    Id = dbSearchCred.Id,
                };
                
                _logger.LogInformation("Токен успешно сгенерирован.");
                
                return Result<UserLoginResponseDto>.Success(responseDto);
            }
            
            _logger.LogInformation("Аутентификация провалена");
            return Result<UserLoginResponseDto>.Failure(
                "Invalid credentials",
                ErrorTypeEnum.AuthenticationError
            );
        }

        public async Task<Result<Guid>> TryUpdateAccount(
            UserUpdateDto user,
            CancellationToken cancellationToken = default
        )
        {
            _logger.LogInformation("Обновление данных аккаунта: {UserId}", user.UserId);
            
            var validationResult = await _validationService.ValidateAsync(user, cancellationToken);
            
            if (!validationResult.IsSuccess)
            {
                _logger.LogWarning("Ошибка валидации при обновлении данных аккаунта: {Error}", validationResult.ErrorMessage);
                return Result<Guid>.Failure(
                    validationResult.ErrorMessage ?? string.Empty,
                    validationResult.ErrorType
                );
            }

            User userModel = _mapper.Map<User>(user);
            
            _logger.LogDebug("Отправлен запрос в репозиторий");
            
            return await _userRepository.Update(userModel, cancellationToken);
        }

        public async Task<Result<Guid>> TryUpdateAccount(
            UserUpdateСredentialsDto user,
            CancellationToken cancellationToken = default
        )
        {
            _logger.LogInformation("Обновление учетных данных аккаунта: {UserId}", user.UserId);
            
            var validationResult = await _validationService.ValidateAsync(user, cancellationToken);

            if (!validationResult.IsSuccess)
            {
                _logger.LogWarning("Ошибка валидации при учетных данных аккаунта: {Error}", validationResult.ErrorMessage);
                return Result<Guid>.Failure(
                    validationResult.ErrorMessage ?? string.Empty,
                    validationResult.ErrorType
                );
            }

            var result = await _userRepository.GetById(user.UserId, cancellationToken);

            if (!result.IsSuccess)
                return Result<Guid>.Failure("Неверный ID", ErrorTypeEnum.NotFound);
            
            _logger.LogDebug("Проверка пароля");
            
            var check = user.OldPassword.VerifyPassword(
                result.Value!.PasswordHash,
                _passwordHasher
            );
            
            if (!check)
            {
                _logger.LogInformation("Неверный пароль для подтверждения обновления");
                return Result<Guid>.Failure("Неверный пароль", ErrorTypeEnum.AuthenticationError);
            }
            
            _logger.LogDebug("Успешное подтверждение, хеширование пароля...");
            
            user.Password = user.Password.HashedPassword(_passwordHasher);

            User userModel = _mapper.Map<User>(user);
            
            _logger.LogDebug("Отправлен запрос в репозиторий");
            
            return await _userRepository.UpdateСredentials(userModel, cancellationToken);
        }

        private bool IsSuccessUserLogin(UserLoginRequest expected, UserLoginResponse actual)
        {
            string passwordHash = actual.PasswordHash;

            return expected.Email == actual.Email
                && expected.Password.VerifyPassword(passwordHash, _passwordHasher);
        }
    }
}
