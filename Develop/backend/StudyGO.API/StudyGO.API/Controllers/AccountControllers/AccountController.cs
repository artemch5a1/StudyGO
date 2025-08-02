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
            UserLoginResponseDto result = await _userAccountService.TryLogIn(loginRequest);

            return result.Success == true ? Ok(result) : BadRequest(result);
        }
    }
}
