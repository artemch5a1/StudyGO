using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyGO.API.Enums;
using StudyGO.API.Extensions;
using StudyGO.Contracts.Dtos.UserProfiles;
using StudyGO.Contracts.PaginationContract;
using StudyGO.Core.Abstractions.Services.Account;
using StudyGO.Core.Extensions;

namespace StudyGO.API.Controllers.UsersControllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserProfileController : ControllerBase
    {
        private readonly ILogger<UserProfileController> _logger;

        private readonly IUserProfileService _userAccountService;

        public UserProfileController(
            ILogger<UserProfileController> logger,
            IUserProfileService userAccountService
        )
        {
            _logger = logger;
            _userAccountService = userAccountService;
        }

        [HttpPost("registry")]
        public async Task<ActionResult<Guid>> RegistryUser(
            [FromBody] UserProfileRegistrDto registryRequest,
            CancellationToken cancellationToken
        )
        {
            _logger.LogInformation("Попытка регистрации пользователя с email: {Email}", 
                LoggingExtensions.MaskEmail(registryRequest.User.Email));
            
            var result = await _userAccountService.TryRegistry(registryRequest, cancellationToken);
            
            _logger.LogResult(result, 
                "Успешная регистрация пользователя", 
                "Ошибка регистрации пользователя",
                new { UserId = result.Value });
            
            return result.ToActionResult();
        }

        [HttpGet("get-all-profiles")]
        [Authorize(Policy = PolicyNames.AdminOnly)]
        public async Task<ActionResult<List<UserProfileDto>>> GetAllProfiles(
            CancellationToken cancellationToken,
            [FromQuery] Pagination paginationParams
        )
        {
            _logger.LogInformation("Запрос всех профилей пользователей");
            
            var result = await _userAccountService.GetAllUserProfiles(cancellationToken, paginationParams);
            
            _logger.LogResult(
                result,
                "Профили успешно получены",
                "Ошибка при получении списка профилей",
                new { CountTeacher = result.Value?.Count }
            );
            
            return result.ToActionResult();
        }

        [HttpGet("get-profile-by-id/{userId}")]
        [Authorize(Policy = PolicyNames.AdminOnly)]
        public async Task<ActionResult<UserProfileDto?>> GetProfileById(
            Guid userId,
            CancellationToken cancellationToken
        )
        {
            _logger.LogInformation("Запрос профиля пользователя по ID: {userId}", userId);
            
            var result = await _userAccountService.TryGetUserProfileById(userId, cancellationToken);
            
            _logger.LogResult(
                result,
                "Профиль найден",
                "Профиль не найден",
                new { UserId = userId }
            );
            
            return result.ToActionResult();
        }

        [HttpGet("get-current-profile")]
        [Authorize]
        public async Task<ActionResult<UserProfileDto?>> GetCurrentProfile(
            CancellationToken cancellationToken
        )
        {
            var userId = User.ExtractGuid();
            
            if (!userId.IsSuccess)
            {
                _logger.LogWarning("Невалидный ID пользователя в токене");
                return BadRequest(userId.ErrorMessage);
            }
            
            _logger.LogDebug("Запрос данных текущего профиля пользователя {UserId}", userId.Value);
            
            var result = await _userAccountService.TryGetUserProfileById(
                userId.Value,
                cancellationToken
            );
            
            _logger.LogResult(
                result,
                "Данные профиля успешно получены",
                "Профиль не найден",
                new { UserId = userId }
            );
            
            return result.ToActionResult();
        }

        [HttpPut("update-profile")]
        [Authorize(Policy = PolicyNames.UserOnly)]
        public async Task<ActionResult<Guid>> UpdateProfile(
            [FromBody] UserProfileUpdateDto userProfile,
            CancellationToken cancellationToken
        )
        {
            if (!User.VerifyGuid(userProfile.UserId))
            {
                _logger.LogWarning("Попытка обновления не своего профиля: {UserId}", userProfile.UserId);
                return Forbid();
            }

            var result = await _userAccountService.TryUpdateUserProfile(
                userProfile,
                cancellationToken
            );
            
            _logger.LogResult(
                result,
                "Профиль успешно обновлён",
                "Ошибка обновления профиля",
                new { userProfile.UserId }
            );
            
            return result.ToActionResult();
        }
    }
}
