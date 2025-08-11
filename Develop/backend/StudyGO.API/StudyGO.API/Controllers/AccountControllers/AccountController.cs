using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyGO.API.Enums;
using StudyGO.API.Extensions;
using StudyGO.Contracts.Contracts;
using StudyGO.Contracts.Dtos.Users;
using StudyGO.Core.Abstractions.Services.Account;
using StudyGO.Core.Extensions;

namespace StudyGO.API.Controllers.AccountControllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly ILogger<AccountController> _logger;

        private readonly IUserAccountService _userAccountService;

        public AccountController(
            ILogger<AccountController> logger,
            IUserAccountService userAccountService
        )
        {
            _logger = logger;
            _userAccountService = userAccountService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserLoginResponseDto>> LoginUser(
            [FromBody] UserLoginRequest loginRequest,
            CancellationToken cancellationToken
        )
        {
            _logger.LogInformation(
                "Попытка входа. Email: {Email}",
                LoggingExtensions.MaskEmail(loginRequest.Email)
            );

            var result = await _userAccountService.TryLogIn(loginRequest, cancellationToken);

            _logger.LogResult(
                result,
                "Успешный вход",
                "Ошибка входа",
                new
                {
                    Email = LoggingExtensions.MaskEmail(loginRequest.Email),
                    UserId = result.Value?.Id,
                }
            );

            return result.ToActionResult();
        }

        [HttpDelete("delete/{userId:guid}")]
        [Authorize(Policy = PolicyNames.AdminOnly)]
        public async Task<ActionResult<Guid>> DeleteUser(
            Guid userId,
            CancellationToken cancellationToken
        )
        {
            _logger.LogInformation("Админ запросил удаление пользователя {UserId}", userId);

            var result = await _userAccountService.TryDeleteAccount(userId, cancellationToken);

            _logger.LogResult(
                result,
                "Пользователь успешно удалён",
                "Ошибка удаления пользователя",
                new { UserId = userId }
            );

            return result.ToActionResult();
        }

        [HttpDelete("delete-current-user")]
        [Authorize]
        public async Task<ActionResult<Guid>> DeleteCurrentUser(CancellationToken cancellationToken)
        {
            var userId = User.ExtractGuid();

            if (!userId.IsSuccess)
            {
                _logger.LogWarning("Не удалось получить ID текущего пользователя");
                return BadRequest(userId.ErrorMessage);
            }

            _logger.LogInformation("Пользователь {userId} запросил удаление своего аккаунта", userId);

            var result = await _userAccountService.TryDeleteAccount(
                userId.Value,
                cancellationToken
            );

            _logger.LogResult(
                result,
                "Пользователь успешно удалён",
                "Ошибка удаления пользователя",
                new { UserId = userId }
            );

            return result.ToActionResult();
        }

        [HttpGet("all-users")]
        [Authorize(Policy = PolicyNames.AdminOnly)]
        public async Task<ActionResult<List<UserDto>>> GetAllUsers(
            CancellationToken cancellationToken
        )
        {
            _logger.LogInformation("Запрос всех пользователей");

            var result = await _userAccountService.TryGetAllAccount(cancellationToken);

            _logger.LogResult(
                result,
                "Пользователи успешно получены",
                "Ошибка при получении списка пользователей",
                new { CountUser = result.Value?.Count }
            );

            return result.ToActionResult();
        }

        [HttpGet("user-by-id/{userId}")]
        [Authorize(Policy = PolicyNames.AdminOnly)]
        public async Task<ActionResult<UserDto?>> GetUserById(
            Guid userId,
            CancellationToken cancellationToken
        )
        {
            _logger.LogInformation("Запрос пользователя по ID: {userId}", userId);

            var result = await _userAccountService.TryGetAccountById(userId, cancellationToken);

            _logger.LogResult(
                result,
                "Пользователь найден",
                "Пользователь не найден",
                new { UserId = userId }
            );

            return result.ToActionResult();
        }

        [HttpGet("current-user")]
        [Authorize]
        public async Task<ActionResult<UserDto?>> GetCurrentUser(
            CancellationToken cancellationToken
        )
        {
            var userId = User.ExtractGuid();

            if (!userId.IsSuccess)
            {
                _logger.LogWarning("Невалидный ID пользователя в токене");
                return BadRequest(userId.ErrorMessage);
            }

            _logger.LogDebug("Запрос данных текущего пользователя {userId}", userId.Value);

            var result = await _userAccountService.TryGetAccountById(
                userId.Value,
                cancellationToken
            );

            _logger.LogResult(
                result,
                "Данные пользователя успешно получены",
                "Пользователь не найден",
                new { UserId = userId }
            );

            return result.ToActionResult();
        }

        [HttpPut("update-user")]
        [Authorize]
        public async Task<ActionResult<Guid>> UpdateUser(
            [FromBody] UserUpdateDto updateDto,
            CancellationToken cancellationToken
        )
        {
            if (!User.VerifyGuid(updateDto.UserId))
            {
                _logger.LogWarning("Попытка обновления не своего аккаунта: {UserId}", updateDto.UserId);
                return Forbid();
            }

            _logger.LogInformation("Обновление пользователя {UserId}", updateDto.UserId);

            var result = await _userAccountService.TryUpdateAccount(updateDto, cancellationToken);

            _logger.LogResult(
                result,
                "Пользователь успешно обновлён",
                "Ошибка обновления пользователя",
                new { updateDto.UserId }
            );

            return result.ToActionResult();
        }

        [HttpPut("update-user-credentials")]
        [Authorize]
        public async Task<ActionResult<Guid>> UpdateCredentials(
            [FromBody] UserUpdateСredentialsDto updateDto,
            CancellationToken cancellationToken
        )
        {
            if (!User.VerifyGuid(updateDto.UserId))
            {
                _logger.LogWarning("Попытка обновления не своего аккаунта: {UserId}", updateDto.UserId);
                return Forbid();
            }

            _logger.LogInformation("Обновление учётных данных пользователя {UserId}", updateDto.UserId);

            var result = await _userAccountService.TryUpdateAccount(updateDto, cancellationToken);

            _logger.LogResult(
                result,
                "Учётные данные пользователя успешно обновлены",
                "Ошибка обновления учётных данных",
                new { updateDto.UserId }
            );

            return result.ToActionResult();
        }
    }
}
