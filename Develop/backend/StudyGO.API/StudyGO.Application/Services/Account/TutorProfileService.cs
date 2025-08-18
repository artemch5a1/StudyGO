using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StudyGO.Application.Extensions;
using StudyGO.Application.Options;
using StudyGO.Contracts.Dtos.TutorProfiles;
using StudyGO.Contracts.PaginationContract;
using StudyGO.Contracts.Result;
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
    public class TutorProfileService : ITutorProfileService
    {
        private readonly ITutorProfileRepository _userRepository;

        private readonly IMapper _mapper;

        private readonly ILogger<TutorProfileService> _logger;

        private readonly IPasswordHasher _passwordHasher;

        private readonly IValidationService _validationService;
        
        private readonly IVerificationService _verificationService;
        
        private readonly TutorProfileServiceOptions _options;
        
        public TutorProfileService(
            ITutorProfileRepository userRepository,
            IMapper mapper,
            ILogger<TutorProfileService> logger,
            IPasswordHasher passwordHasher,
            IValidationService validationService,
            IOptions<TutorProfileServiceOptions> options, 
            IVerificationService verificationService)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _logger = logger;
            _passwordHasher = passwordHasher;
            _validationService = validationService;
            _verificationService = verificationService;
            _options = options.Value;
        }

        public async Task<Result<List<TutorProfileDto>>> GetAllUserProfiles(
            CancellationToken cancellationToken = default,
            Pagination? value = null
        )
        {
            _logger.LogInformation("Получение всех профилей учителей");
            
            var result = value == null ? await _userRepository.GetAll(cancellationToken) : 
                await _userRepository.GetPages(value.Skip, value.Take, cancellationToken);
            
            _logger.LogDebug("Получено {Count} профилей учителей", result.Value?.Count ?? 0);
            
            return result.MapDataTo(_mapper.Map<List<TutorProfileDto>>);
        }

        public async Task<Result<List<TutorProfileDto>>> GetAllUserVerifiedProfiles(
            CancellationToken cancellationToken = default,
            Pagination? value = null
        )
        {
            _logger.LogInformation("Получение всех подтвержденных профилей учителей");
            
            var result = value == null ? await _userRepository.GetAllVerified(cancellationToken) : 
                await _userRepository.GetPages(value.Skip, value.Take, cancellationToken);
            
            _logger.LogDebug("Получено {Count} профилей учителей", result.Value?.Count ?? 0);
            
            return result.MapDataTo(_mapper.Map<List<TutorProfileDto>>);
        }

        public async Task<Result<TutorProfileDto?>> TryGetUserProfileById(
            Guid userId,
            CancellationToken cancellationToken = default
        )
        {
            _logger.LogInformation("Поиск профиля учителя по ID: {UserId}", userId);
            
            var result = await _userRepository.GetById(userId, cancellationToken);
            
            _logger.LogResult(result, 
                "Профиль учителя найден", 
                "Профиль учителя не найден", 
                new { UserId = userId });
            
            return result.MapDataTo(_mapper.Map<TutorProfileDto?>);
        }

        public async Task<Result<TutorProfileDto?>> TryGetVerifiedUserProfileById(
            Guid userId,
            CancellationToken cancellationToken = default
        )
        {
            _logger.LogInformation("Поиск профиля подтвержденного учителя по ID: {UserId}", userId);
            
            var result = await _userRepository.GetByIdVerified(userId, cancellationToken);
            
            _logger.LogResult(result, 
                "Профиль учителя найден", 
                "Профиль учителя не найден", 
                new { UserId = userId });
            
            return result.MapDataTo(_mapper.Map<TutorProfileDto?>);
        }

        public async Task<Result<Guid>> TryRegistry(
            TutorProfileRegistrDto profile,
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
            TutorProfileUpdateDto newProfile,
            CancellationToken cancellationToken = default
        )
        {
            _logger.LogInformation("Обновление профиля учителя: {UserId}", newProfile.UserId);
            
            var validatorResult = await _validationService.ValidateAsync(
                newProfile,
                cancellationToken
            );

            if (!validatorResult.IsSuccess)
            {
                _logger.LogWarning("Ошибка валидации при обновлении профиля учителя: {Error}", validatorResult.ErrorMessage);
                return Result<Guid>.Failure(
                    validatorResult.ErrorMessage ?? string.Empty,
                    validatorResult.ErrorType
                );
            }
            
            _logger.LogDebug("Отправлен запрос в репозиторий");
            
            return await _userRepository.Update(
                _mapper.Map<TutorProfile>(newProfile),
                cancellationToken
            );
        }

        private async Task<Result<Guid>> RegistryLogic(TutorProfileRegistrDto profile,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Попытка регистрации учителя с email: {Email}", 
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
            
            TutorProfile profileModel = _mapper.Map<TutorProfile>(profile);
            
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
        
        private async Task<Result<Guid>> DefaultConfirm(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _userRepository.DefaultVerification(userId, cancellationToken);
        }
    }
}
