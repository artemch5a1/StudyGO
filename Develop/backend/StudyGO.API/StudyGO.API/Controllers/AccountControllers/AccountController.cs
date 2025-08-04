using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyGO.API.Enums;
using StudyGO.Contracts.Contracts;
using StudyGO.Contracts.Dtos.Users;
using StudyGO.Core.Abstractions.Services.Account;
using StudyGO.API.Extensions;

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
            var userId = User.ExtractGuid();

            if (!userId.IsSuccess)
                return BadRequest(userId.ErrorMessage);

            var result = await _userAccountService.TryDeleteAccount(userId.Value);

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
            var userId = User.ExtractGuid();

            if (!userId.IsSuccess)
                return BadRequest(userId.ErrorMessage);

            var result = await _userAccountService.TryGetAccountById(userId.Value);

            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.ErrorMessage);
        }

        [HttpPut("update-user")]
        [Authorize]
        public async Task<ActionResult<Guid>> UpdateUser([FromBody] UserUpdateDto updateDto)
        {
            if (!User.VerifyGuid(updateDto.UserID))
                return Unauthorized("Äîñòóï çàïðåùåí");

            var result = await _userAccountService.TryUpdateAccount(updateDto);

            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.ErrorMessage);
        }

        [HttpPut("update-user-credentials")]
        [Authorize]
        public async Task<ActionResult<Guid>> UpdateCredentials(
            [FromBody] UserUpdateÑredentialsDto updateDto
        )
        {
            if (!User.VerifyGuid(updateDto.UserId))
                return Unauthorized("Äîñòóï çàïðåùåí");

            var result = await _userAccountService.TryUpdateAccount(updateDto);

            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.ErrorMessage);
        }
    }
}
