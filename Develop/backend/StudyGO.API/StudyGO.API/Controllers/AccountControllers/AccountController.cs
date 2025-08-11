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
                "������� �����. Email: {Email}",
                LoggingExtensions.MaskEmail(loginRequest.Email)
            );

            var result = await _userAccountService.TryLogIn(loginRequest, cancellationToken);

            _logger.LogResult(
                result,
                "�������� ����",
                "������ �����",
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
            _logger.LogInformation("����� �������� �������� ������������ {UserId}", userId);

            var result = await _userAccountService.TryDeleteAccount(userId, cancellationToken);

            _logger.LogResult(
                result,
                "������������ ������� �����",
                "������ �������� ������������",
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
                _logger.LogWarning("�� ������� �������� ID �������� ������������");
                return BadRequest(userId.ErrorMessage);
            }

            _logger.LogInformation("������������ {userId} �������� �������� ������ ��������", userId);

            var result = await _userAccountService.TryDeleteAccount(
                userId.Value,
                cancellationToken
            );

            _logger.LogResult(
                result,
                "������������ ������� �����",
                "������ �������� ������������",
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
            _logger.LogInformation("������ ���� �������������");

            var result = await _userAccountService.TryGetAllAccount(cancellationToken);

            _logger.LogResult(
                result,
                "������������ ������� ��������",
                "������ ��� ��������� ������ �������������",
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
            _logger.LogInformation("������ ������������ �� ID: {userId}", userId);

            var result = await _userAccountService.TryGetAccountById(userId, cancellationToken);

            _logger.LogResult(
                result,
                "������������ ������",
                "������������ �� ������",
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
                _logger.LogWarning("���������� ID ������������ � ������");
                return BadRequest(userId.ErrorMessage);
            }

            _logger.LogDebug("������ ������ �������� ������������ {userId}", userId.Value);

            var result = await _userAccountService.TryGetAccountById(
                userId.Value,
                cancellationToken
            );

            _logger.LogResult(
                result,
                "������ ������������ ������� ��������",
                "������������ �� ������",
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
                _logger.LogWarning("������� ���������� �� ������ ��������: {UserId}", updateDto.UserId);
                return Forbid();
            }

            _logger.LogInformation("���������� ������������ {UserId}", updateDto.UserId);

            var result = await _userAccountService.TryUpdateAccount(updateDto, cancellationToken);

            _logger.LogResult(
                result,
                "������������ ������� �������",
                "������ ���������� ������������",
                new { updateDto.UserId }
            );

            return result.ToActionResult();
        }

        [HttpPut("update-user-credentials")]
        [Authorize]
        public async Task<ActionResult<Guid>> UpdateCredentials(
            [FromBody] UserUpdate�redentialsDto updateDto,
            CancellationToken cancellationToken
        )
        {
            if (!User.VerifyGuid(updateDto.UserId))
            {
                _logger.LogWarning("������� ���������� �� ������ ��������: {UserId}", updateDto.UserId);
                return Forbid();
            }

            _logger.LogInformation("���������� ������� ������ ������������ {UserId}", updateDto.UserId);

            var result = await _userAccountService.TryUpdateAccount(updateDto, cancellationToken);

            _logger.LogResult(
                result,
                "������� ������ ������������ ������� ���������",
                "������ ���������� ������� ������",
                new { updateDto.UserId }
            );

            return result.ToActionResult();
        }
    }
}
