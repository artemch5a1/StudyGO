using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyGO.API.Enums;
using StudyGO.API.Extensions;
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

        [HttpPost("registr")]
        public async Task<ActionResult<Guid>> RegistrUser(
            [FromBody] UserProfileRegistrDto registrRequest
        )
        {
            var result = await _userAccountService.TryRegistr(registrRequest);

            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.ErrorMessage);
        }

        [HttpGet("get-all-profiles")]
        [Authorize(Policy = PolicyNames.AdminOnly)]
        public async Task<ActionResult<List<UserProfileDto>>> GetAllProfiles()
        {
            var result = await _userAccountService.GetAllUserProfiles();

            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.ErrorMessage);
        }

        [HttpGet("get-profile-by-id/{userID}")]
        [Authorize(Policy = PolicyNames.AdminOnly)]
        public async Task<ActionResult<UserProfileDto>> GetProfileById(Guid userID)
        {
            var result = await _userAccountService.TryGetUserProfileById(userID);

            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.ErrorMessage);
        }

        [HttpGet("get-current-profile")]
        [Authorize]
        public async Task<ActionResult<UserProfileDto>> GetCurrentProfile()
        {
            var userID = User.ExtractGuid();

            if (!userID.IsSuccess)
                return BadRequest(userID.ErrorMessage);

            var result = await _userAccountService.TryGetUserProfileById(userID.Value);

            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.ErrorMessage);
        }

        [HttpPut("update-profile")]
        [Authorize(Policy = PolicyNames.UserOnly)]
        public async Task<ActionResult<Guid>> UpdateProfile(
            [FromBody] UserProfileUpdateDto userProfile
        )
        {
            if (!User.VerifyGuid(userProfile.UserID))
                return Unauthorized("Доступ запрещен");

            var result = await _userAccountService.TryUpdateUserProfile(userProfile);

            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.ErrorMessage);
        }
    }
}
