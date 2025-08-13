using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StudyGO.Application.Extensions;
using StudyGO.Contracts.Dtos.UserProfiles;
using StudyGO.Contracts.PaginationContract;
using StudyGO.Contracts.Result;
using StudyGO.Core.Abstractions.Auth;
using StudyGO.Core.Abstractions.EmailServices;
using StudyGO.Core.Abstractions.Repositories;
using StudyGO.Core.Abstractions.Services.Account;
using StudyGO.Core.Abstractions.Utils;
using StudyGO.Core.Abstractions.ValidationService;
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
        
        private readonly IEmailVerifyTokenProvider _emailTokenProvider;

        private readonly IEmailService _emailService;

        private readonly IConfiguration _configuration;

        public UserProfileService(
            IUserProfileRepository userRepository,
            IMapper mapper,
            ILogger<UserProfileService> logger,
            IPasswordHasher passwordHasher,
            IValidationService validationService,
            IEmailVerifyTokenProvider emailTokenProvider,
            IEmailService emailService,
            IConfiguration configuration
        )
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _logger = logger;
            _passwordHasher = passwordHasher;
            _validationService = validationService;
            _emailTokenProvider = emailTokenProvider;
            _emailService = emailService;
            _configuration = configuration;
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
            CancellationToken cancellationToken = default
        )
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

            var resultToken = await CreateTokenAndSendMessage(profileModel.UserId, 
                profileModel.User?.Email ?? "", 
                cancellationToken);

            if(!resultToken.IsSuccess)
            {
                return Result<Guid>.Failure(resultToken.ErrorMessage ?? "", resultToken.ErrorType);
            }

            profileModel.User!.VerifiedToken = resultToken.Value;
            
            _logger.LogDebug("Отправлен запрос в репозиторий");
            
            return await _userRepository.Create(profileModel, cancellationToken);
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
        
        private async Task<Result<string>> CreateTokenAndSendMessage(Guid userId, string email, CancellationToken cancellationToken)
        {
            string token = _emailTokenProvider.GenerateToken(userId);
            
            var baseUrl = _configuration["EmailSettings:BaseUrl"] 
                          ?? throw new InvalidOperationException("BaseUrl не настроен в конфигурации");
            
            var verificationLink = $"{baseUrl.TrimEnd('/')}/VerifyEmail?userId={userId}&token={Uri.EscapeDataString(token)}";
            
            var result = await _emailService.SendVerificationEmailAsync(email, 
                verificationLink, 
                "Подтверждение email", cancellationToken);

            return result.IsSuccess ? 
                Result<string>.Success(token) : 
                Result<string>.Failure(result.ErrorMessage ?? "", result.ErrorType);
        }
    }
}
