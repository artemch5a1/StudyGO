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

        [HttpPost("registry")]
        public async Task<ActionResult<Guid>> RegistryUser(
            [FromBody] UserProfileRegistrDto registryRequest,
            CancellationToken cancellationToken
        )
        {
            var result = await _userAccountService.TryRegistr(registryRequest, cancellationToken);

            return result.ToActionResult();
        }

        [HttpGet("get-all-profiles")]
        [Authorize(Policy = PolicyNames.AdminOnly)]
        public async Task<ActionResult<List<UserProfileDto>>> GetAllProfiles(
            CancellationToken cancellationToken
        )
        {
            var result = await _userAccountService.GetAllUserProfiles(cancellationToken);

            return result.ToActionResult();
        }

        [HttpGet("get-profile-by-id/{userId}")]
        [Authorize(Policy = PolicyNames.AdminOnly)]
        public async Task<ActionResult<UserProfileDto?>> GetProfileById(
            Guid userId,
            CancellationToken cancellationToken
        )
        {
            var result = await _userAccountService.TryGetUserProfileById(userId, cancellationToken);

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
                return BadRequest(userId.ErrorMessage);

            var result = await _userAccountService.TryGetUserProfileById(
                userId.Value,
                cancellationToken
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
                return Forbid();

            var result = await _userAccountService.TryUpdateUserProfile(
                userProfile,
                cancellationToken
            );

            return result.ToActionResult();
        }
    }
}
