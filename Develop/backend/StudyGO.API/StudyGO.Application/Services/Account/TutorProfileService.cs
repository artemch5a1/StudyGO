using System.Threading.Channels;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StudyGO.Application.Extensions;
using StudyGO.Application.Options;
using StudyGO.Contracts.Contracts;
using StudyGO.Contracts.Dtos.TutorProfiles;
using StudyGO.Contracts.PaginationContract;
using StudyGO.Contracts.Result;
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
        
        private readonly Channel<VerificationJob> _verificationQueue;
        
        public TutorProfileService(
            ITutorProfileRepository userRepository,
            IMapper mapper,
            ILogger<TutorProfileService> logger,
            IPasswordHasher passwordHasher,
            IValidationService validationService,
            IOptionsSnapshot<TutorProfileServiceOptions> options, 
            Channel<VerificationJob> verificationQueue)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _logger = logger;
            _passwordHasher = passwordHasher;
            _validationService = validationService;
            _verificationQueue = verificationQueue;
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
            
            if (!resultCreate.IsSuccess)
                return resultCreate;

            if (_options.RequireEmailVerification)
            {
                var job = new VerificationJob(resultCreate.Value, profile.User.Email, confirmEmailEndpoint);
                await _verificationQueue.Writer.WriteAsync(job, cancellationToken);
                return resultCreate;
            }

            return await DefaultConfirm(resultCreate.Value, cancellationToken);
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
