using Microsoft.AspNetCore.Mvc;
using StudyGO.Contracts.Contracts;
using StudyGO.Contracts.Dtos.Users;
using StudyGO.Core.Abstractions.Services.Account;

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

        [HttpPost(Name = "login")]
        public async Task<ActionResult<UserLoginResponseDto>> LoginUser(
            [FromBody] UserLoginRequest loginRequest
        )
        {
            var result = await _userAccountService.TryLogIn(loginRequest);

            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.ErrorMessage);
        }

        [HttpDelete("delete/{userID}")]
        public async Task<ActionResult<Guid>> DeleteUser(Guid userID)
        {
            var result = await _userAccountService.TryDeleteAccount(userID);

            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.ErrorMessage);
        }

        [HttpGet("all-users")]
        public async Task<ActionResult<List<UserDto>>> GetAllUsers()
        {
            var result = await _userAccountService.TryGetAllAccount();

            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.ErrorMessage);
        }

        [HttpGet("user-by-id/{userID}")]
        public async Task<ActionResult<UserDto?>> GetUserById(Guid userID)
        {
            var result = await _userAccountService.TryGetAccountById(userID);

            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.ErrorMessage);
        }

        [HttpPut("update-user")]
        public async Task<ActionResult<Guid>> UpdateUser([FromBody] UserUpdateDto updateDto)
        {
            var result = await _userAccountService.TryUpdateAccount(updateDto);

            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.ErrorMessage);
        }
    }
}
