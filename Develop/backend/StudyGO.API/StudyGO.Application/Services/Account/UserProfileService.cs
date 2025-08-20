using System.Threading.Channels;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StudyGO.Application.Extensions;
using StudyGO.Application.Options;
using StudyGO.Contracts.Contracts;
using StudyGO.Contracts.Dtos.UserProfiles;
using StudyGO.Contracts.PaginationContract;
using StudyGO.Contracts.Result;
using StudyGO.Core.Abstractions.Contracts;
using StudyGO.Core.Abstractions.Repositories;
using StudyGO.Core.Abstractions.Services.Account;
using StudyGO.Core.Abstractions.Utils;
using StudyGO.Core.Abstractions.ValidationService;
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

        private readonly UserProfileServiceOptions _options;
        
        private readonly IVerificationJobQueue _verificationQueue;
        
        public UserProfileService(
            IUserProfileRepository userRepository,
            IMapper mapper,
            ILogger<UserProfileService> logger,
            IPasswordHasher passwordHasher,
            IValidationService validationService, 
            IOptionsSnapshot<UserProfileServiceOptions> options, 
            IVerificationJobQueue verificationQueue)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _logger = logger;
            _passwordHasher = passwordHasher;
            _validationService = validationService;
            _verificationQueue = verificationQueue;
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
        
        public async Task<Result<List<UserProfileDto>>> GetAllUserVerifiedProfiles(
            CancellationToken cancellationToken = default,
            Pagination? value = null
        )
        {
            _logger.LogInformation("Получение всех подтвержденных профилей пользователей");
            
            var result = value == null ? await _userRepository.GetAllVerified(cancellationToken) 
                : await _userRepository.GetPagesVerified(value.Skip, value.Take,
                cancellationToken);
            
            _logger.LogDebug("Получено {Count} подтвержденных профилей пользователей", result.Value?.Count ?? 0);
            
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
        
        public async Task<Result<UserProfileDto?>> TryGetVerifiedUserProfileById(
            Guid userId,
            CancellationToken cancellationToken = default
        )
        {
            _logger.LogInformation("Поиск профиля пользователя по ID: {UserId}", userId);
            
            var result = await _userRepository.GetByIdVerified(userId, cancellationToken);
            
            _logger.LogResult(result, 
                "Подтвержденный профиль пользователя найден", 
                "Профиль пользователя не найден среди подтвержденных", 
                new { UserId = userId });
            
            return result.MapDataTo(_mapper.Map<UserProfileDto?>);
        }
        
        public async Task<Result<UserRegistryResponse>> TryRegistry(
            UserProfileRegistrDto profile,
            string confirmEmailEndpoint,
            CancellationToken cancellationToken = default
        )
        {
            var resultCreate = await RegistryLogic(profile, cancellationToken);

            if (!resultCreate.IsSuccess)
                return resultCreate.MapDataTo(UserRegistryResponse.WithoutVerified);

            if (_options.RequireEmailVerification)
            {
                var job = new VerificationJob(resultCreate.Value, profile.User.Email, confirmEmailEndpoint);
                await _verificationQueue.EnqueueAsync(job, cancellationToken);
                return resultCreate.MapDataTo(UserRegistryResponse.VerifiedByLink);
            }

            var defaultConfirm = await DefaultConfirm(resultCreate.Value, cancellationToken);

            return defaultConfirm.MapDataTo(UserRegistryResponse.WithoutVerified);
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

        private async Task<Result<Guid>> DefaultConfirm(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _userRepository.DefaultVerification(userId, cancellationToken);
        }
    }
}
