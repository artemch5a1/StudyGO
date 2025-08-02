using Microsoft.AspNetCore.Mvc;
using StudyGO.Contracts.Dtos.UserProfiles;
using StudyGO.Core.Abstractions.Services.Account;

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

        [HttpPost(Name = "registr")]
        public async Task<ActionResult<Guid>> RegistrUser(
            [FromBody] UserProfileRegistrDto registrRequest
        )
        {
            var result = await _userAccountService.TryRegistr(registrRequest);

            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.ErrorMessage);
        }
    }
}
