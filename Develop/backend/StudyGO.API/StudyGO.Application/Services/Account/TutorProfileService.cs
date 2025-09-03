using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StudyGO.Application.Extensions;
using StudyGO.Application.Options;
using StudyGO.Contracts.Contracts;
using StudyGO.Contracts.Dtos.TutorProfiles;
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
    public class TutorProfileService : ITutorProfileService
    {
        private readonly ITutorProfileRepository _userRepository;

        private readonly IMapper _mapper;

        private readonly ILogger<TutorProfileService> _logger;

        private readonly IPasswordHasher _passwordHasher;

        private readonly IValidationService _validationService;
        
        private readonly TutorProfileServiceOptions _options;
        
        private readonly IVerificationJobQueue _verificationQueue;
        
        public TutorProfileService(
            ITutorProfileRepository userRepository,
            IMapper mapper,
            ILogger<TutorProfileService> logger,
            IPasswordHasher passwordHasher,
            IValidationService validationService,
            IOptionsSnapshot<TutorProfileServiceOptions> options, 
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

        public async Task<Result<UserRegistryResponse>> TryRegistry(
            TutorProfileRegistrDto profile,
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
        
        private async Task<Result<Guid>> DefaultConfirm(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _userRepository.DefaultVerification(userId, cancellationToken);
        }
    }
}
