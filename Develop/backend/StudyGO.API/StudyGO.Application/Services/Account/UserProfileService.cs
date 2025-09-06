using AutoMapper;
using Microsoft.Extensions.Logging;
using StudyGO.Contracts.Dtos.UserProfiles;
using StudyGO.Contracts.PaginationContract;
using StudyGO.Contracts.Result;
using StudyGO.Core.Abstractions.Repositories;
using StudyGO.Core.Abstractions.Services.Account;
using StudyGO.Core.Extensions;

namespace StudyGO.Application.Services.Account
{
    public class UserProfileService : IUserProfileService
    {
        private readonly IUserProfileRepository _userRepository;

        private readonly IMapper _mapper;

        private readonly ILogger<UserProfileService> _logger;
        
        public UserProfileService(
            IUserProfileRepository userRepository,
            IMapper mapper,
            ILogger<UserProfileService> logger)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _logger = logger;
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
    }
}
