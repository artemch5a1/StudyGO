using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Win32;
using StudyGO.Application.Extensions;
using StudyGO.Application.Options;
using StudyGO.Contracts.Dtos.UserProfiles;
using StudyGO.Contracts.PaginationContract;
using StudyGO.Contracts.Result;
using StudyGO.Core.Abstractions.Auth;
using StudyGO.Core.Abstractions.EmailServices;
using StudyGO.Core.Abstractions.Repositories;
using StudyGO.Core.Abstractions.Services.Account;
using StudyGO.Core.Abstractions.Utils;
using StudyGO.Core.Abstractions.ValidationService;
using StudyGO.Core.Abstractions.Verification;
using StudyGO.Core.Enums;
using StudyGO.Core.Extensions;
using StudyGO.Core.Models;

namespace StudyGO.Application.Services.Account
{
    public class UserProfileService : IUserProfileService
    {
        private readonly IUserProfileRepository _userRepository;

        private readonly IMapper _mapper;

        private readonly ILogger<UserProfileService> _logger;

        private readonly IPasswordHasher _passwordHasher;

        private readonly IValidationService _validationService;

        private readonly IVerificationService _verificationService;

        private readonly UserProfileServiceOptions _options;
        
        public UserProfileService(
            IUserProfileRepository userRepository,
            IMapper mapper,
            ILogger<UserProfileService> logger,
            IPasswordHasher passwordHasher,
            IValidationService validationService, 
            IVerificationService verificationService, 
            IOptions<UserProfileServiceOptions> options)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _logger = logger;
            _passwordHasher = passwordHasher;
            _validationService = validationService;
            _verificationService = verificationService;
            _options = options.Value;
        }

        public async Task<Result<List<UserProfileDto>>> GetAllUserProfiles(
            CancellationToken cancellationToken = default,
            Pagination? value = null
        )
        {
            _logger.LogInformation("Получение всех профилей пользователей");
            
            var result = value == null ? await _userRepository.GetAll(cancellationToken) : await _userRepository.GetPages(value.Skip, value.Take,
                cancellationToken);
            
            _logger.LogDebug("Получено {Count} профилей пользователей", result.Value?.Count ?? 0);
            
            return result.MapDataTo(_mapper.Map<List<UserProfileDto>>);
        }

        public async Task<Result<UserProfileDto?>> TryGetUserProfileById(
            Guid userId,
            CancellationToken cancellationToken = default
        )
        {
            _logger.LogInformation("Поиск профиля пользователя по ID: {UserId}", userId);
            
            var result = await _userRepository.GetById(userId, cancellationToken);
            
            _logger.LogResult(result, 
                "Профиль пользователя найден", 
                "Профиль пользователя не найден", 
                new { UserId = userId });
            
            return result.MapDataTo(_mapper.Map<UserProfileDto?>);
        }

        public async Task<Result<Guid>> TryRegistry(
            UserProfileRegistrDto profile,
            string confirmEmailEndpoint,
            CancellationToken cancellationToken = default
        )
        {
            var resultCreate = await RegistryLogic(profile, cancellationToken);

            return await VerificationLogic(
                resultCreate,
                confirmEmailEndpoint,
                profile.User.Email,
                cancellationToken
            );
        }

        public async Task<Result<Guid>> TryUpdateUserProfile(
            UserProfileUpdateDto newProfile,
            CancellationToken cancellationToken = default
        )
        {
            _logger.LogInformation("Обновление профиля пользователя: {UserId}", newProfile.UserId);
            
            var validatorResult = await _validationService.ValidateAsync(
                newProfile,
                cancellationToken
            );

            if (!validatorResult.IsSuccess)
            {
                _logger.LogWarning("Ошибка валидации при обновлении профиля пользователя: {Error}", validatorResult.ErrorMessage);
                return Result<Guid>.Failure(
                    validatorResult.ErrorMessage ?? string.Empty,
                    validatorResult.ErrorType
                );
            }

            UserProfile user = _mapper.Map<UserProfile>(newProfile);
            
            _logger.LogDebug("Отправлен запрос в репозиторий");
            
            return await _userRepository.Update(user, cancellationToken);
        }

        private async Task<Result<Guid>> RegistryLogic(UserProfileRegistrDto profile,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Попытка регистрации пользователя с email: {Email}", 
                LoggingExtensions.MaskEmail(profile.User.Email));
            
            var validatorResult = await _validationService.ValidateAsync(
                profile,
                cancellationToken
            );

            if (!validatorResult.IsSuccess)
            {
                _logger.LogWarning("Ошибка валидации при регистрации учителя: {Error}", validatorResult.ErrorMessage);
                return Result<Guid>.Failure(
                    validatorResult.ErrorMessage ?? string.Empty,
                    validatorResult.ErrorType
                );
            }

            _logger.LogDebug("Валидация прошла успешно. Хеширование пароля...");
            profile.User.Password = profile.User.Password.HashedPassword(_passwordHasher);
            
            _logger.LogDebug("Маппинг...");
            
            UserProfile profileModel = _mapper.Map<UserProfile>(profile);
            
            _logger.LogDebug("Отправлен запрос в репозиторий");
            
            return await _userRepository.Create(profileModel, cancellationToken);
        }

        private async Task<Result<Guid>> VerificationLogic(
            Result<Guid> resultCreate, 
            string confirmEmailEndpoint,
            string email,
            CancellationToken cancellationToken = default
            )
        {
            if (!resultCreate.IsSuccess)
            {
                return resultCreate;
            }

            if(_options.RequireEmailVerification)
            {
                return await SmtpConfirm(
                    resultCreate, 
                    confirmEmailEndpoint,
                    email,
                    cancellationToken
                    );
            }

            return await DefaultConfirm(resultCreate.Value, cancellationToken);
        }

        private async Task<Result<Guid>> DefaultConfirm(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _userRepository.DefaultVerification(userId, cancellationToken);
        }

        private async Task<Result<Guid>> SmtpConfirm(
            Result<Guid> resultCreate, 
            string confirmEmailEndpoint,
            string email,
            CancellationToken cancellationToken = default
            )
        {
            var resultToken = await _verificationService.CreateTokenAndSendMessage(
                resultCreate.Value, 
                email,
                confirmEmailEndpoint, 
                cancellationToken);
            
            if(!resultToken.IsSuccess)
            {
                return Result<Guid>.Failure(resultToken.ErrorMessage ?? "", resultToken.ErrorType);
            }

            return resultCreate;
        }
    }
}
