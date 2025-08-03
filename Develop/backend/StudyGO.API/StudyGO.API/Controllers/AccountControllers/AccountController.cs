using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyGO.API.Enums;
using StudyGO.Contracts.Contracts;
using StudyGO.Contracts.Dtos.Users;
using StudyGO.Core.Abstractions.Services.Account;
using StudyGO.Core.Models;
using System.Security.Claims;

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
            [FromBody] UserLoginRequest loginRequest
        )
        {
            var result = await _userAccountService.TryLogIn(loginRequest);

            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.ErrorMessage);
        }

        [HttpDelete("delete/{userID}")]
        [Authorize(Policy = PolicyNames.AdminOnly)]
        public async Task<ActionResult<Guid>> DeleteUser(Guid userID)
        {
            var result = await _userAccountService.TryDeleteAccount(userID);

            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.ErrorMessage);
        }

        [HttpDelete("delete-current-user")]
        [Authorize]
        public async Task<ActionResult<Guid>> DeleteCurrentUser()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
            {
                return Unauthorized("Invalid user ID in token");
            }

            var result = await _userAccountService.TryDeleteAccount(userGuid);

            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.ErrorMessage);
        }

        [HttpGet("all-users")]
        [Authorize(Policy = PolicyNames.AdminOnly)]
        public async Task<ActionResult<List<UserDto>>> GetAllUsers()
        {
            var result = await _userAccountService.TryGetAllAccount();

            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.ErrorMessage);
        }

        [HttpGet("user-by-id/{userID}")]
        [Authorize(Policy = PolicyNames.AdminOnly)]
        public async Task<ActionResult<UserDto?>> GetUserById(Guid userID)
        {
            var result = await _userAccountService.TryGetAccountById(userID);

            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.ErrorMessage);
        }

        [HttpGet("current-user")]
        [Authorize]
        public async Task<ActionResult<UserDto?>> GetCurrentUser()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
            {
                return Unauthorized("Invalid user ID in token");
            }

            var result = await _userAccountService.TryGetAccountById(userGuid);

            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.ErrorMessage);
        }

        [HttpPut("update-user")]
        [Authorize]
        public async Task<ActionResult<Guid>> UpdateUser([FromBody] UserUpdateDto updateDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
            {
                return Unauthorized("Invalid user ID in token");
            }

            if (userGuid != updateDto.UserID)
            {
                return Unauthorized("Попытка обновить другого пользователя");
            }

            var result = await _userAccountService.TryUpdateAccount(updateDto);

            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.ErrorMessage);
        }

        [HttpPut("update-user-credentials")]
        [Authorize]
        public async Task<ActionResult<Guid>> UpdateCredentials(
            [FromBody] UserUpdateСredentialsDto updateDto
        )
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
            {
                return Unauthorized("Invalid user ID in token");
            }

            if (userGuid != updateDto.UserId)
            {
                return Unauthorized("Попытка обновить другого пользователя");
            }

            var result = await _userAccountService.TryUpdateAccount(updateDto);

            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.ErrorMessage);
        }
    }
}
