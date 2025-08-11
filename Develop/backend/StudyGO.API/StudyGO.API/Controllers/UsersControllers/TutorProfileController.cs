using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyGO.API.Enums;
using StudyGO.API.Extensions;
using StudyGO.Contracts.Dtos.TutorProfiles;
using StudyGO.Contracts.PaginationContract;
using StudyGO.Core.Abstractions.Services.Account;
using StudyGO.Core.Extensions;

namespace StudyGO.API.Controllers.UsersControllers
{
    [ApiController]
    [Route("[controller]")]
    public class TutorProfileController : ControllerBase
    {
        private readonly ILogger<TutorProfileController> _logger;

        private readonly ITutorProfileService _tutorAccountService;

        public TutorProfileController(
            ILogger<TutorProfileController> logger,
            ITutorProfileService userAccountService
        )
        {
            _logger = logger;
            _tutorAccountService = userAccountService;
        }

        [HttpPost("registry")]
        public async Task<ActionResult<Guid>> RegistryUser(
            [FromBody] TutorProfileRegistrDto registryRequest,
            CancellationToken cancellationToken
        )
        {
            _logger.LogInformation("Попытка регистрации учителя с email: {Email}", 
                LoggingExtensions.MaskEmail(registryRequest.User.Email));
            
            var result = await _tutorAccountService.TryRegistry(registryRequest, cancellationToken);
            
            _logger.LogResult(result, 
                "Успешная регистрация учителя", 
                "Ошибка регистрации учителя",
                new { UserId = result.Value });
            
            return result.ToActionResult();
        }

        [HttpGet("get-all-profiles")]
        [Authorize]
        public async Task<ActionResult<List<TutorProfileDto>>> GetAllProfiles(
            [FromQuery] Pagination paginationParams,
            CancellationToken cancellationToken
        )
        {
            _logger.LogInformation("Запрос всех учителей");
            
            var result = await _tutorAccountService.GetAllUserProfiles(cancellationToken, paginationParams);
            
            _logger.LogResult(
                result,
                "Учителя успешно получены",
                "Ошибка при получении списка учителей",
                new { CountTeacher = result.Value?.Count }
            );
            
            return result.ToActionResult();
        }

        [HttpGet("get-profile-by-id/{userId:guid}")]
        [Authorize(Policy = PolicyNames.UserOrAdmin)]
        public async Task<ActionResult<TutorProfileDto?>> GetProfileById(
            Guid userId,
            CancellationToken cancellationToken
        )
        {
            _logger.LogInformation("Запрос учителя по ID: {userId}", userId);
            
            var result = await _tutorAccountService.TryGetUserProfileById(
                userId,
                cancellationToken
            );
            
            _logger.LogResult(
                result,
                "Учитель найден",
                "Учитель не найден",
                new { UserId = userId }
            );
            
            return result.ToActionResult();
        }

        [HttpGet("get-current-profile")]
        [Authorize]
        public async Task<ActionResult<TutorProfileDto?>> GetCurrentUser(
            CancellationToken cancellationToken
        )
        {
            var userId = User.ExtractGuid();

            if (!userId.IsSuccess)
            {
                _logger.LogWarning("Невалидный ID пользователя в токене");
                return BadRequest(userId.ErrorMessage);
            }
            
            _logger.LogDebug("Запрос данных текущего учителя {userId}", userId.Value);
            
            var result = await _tutorAccountService.TryGetUserProfileById(
                userId.Value,
                cancellationToken
            );
            
            _logger.LogResult(
                result,
                "Данные учителя успешно получены",
                "Учитель не найден",
                new { UserId = userId }
            );
            
            return result.ToActionResult();
        }

        [HttpPut("update-profile")]
        [Authorize(Policy = PolicyNames.TutorOnly)]
        public async Task<ActionResult<Guid>> UpdateProfile(
            [FromBody] TutorProfileUpdateDto userProfile,
            CancellationToken cancellationToken
        )
        {
            if (!User.VerifyGuid(userProfile.UserId))
            {
                _logger.LogWarning("Попытка обновления не своего аккаунта: {UserId}", userProfile.UserId);
                return Forbid();
            }
            
            _logger.LogInformation("Обновление учителя {UserId}", userProfile.UserId);
            
            var result = await _tutorAccountService.TryUpdateUserProfile(
                userProfile,
                cancellationToken
            );
            
            _logger.LogResult(
                result,
                "Учитель успешно обновлён",
                "Ошибка обновления учителя",
                new { userProfile.UserId }
            );
            
            return result.ToActionResult();
        }
    }
}
