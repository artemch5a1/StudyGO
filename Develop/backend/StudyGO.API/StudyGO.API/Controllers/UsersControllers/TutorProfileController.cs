using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyGO.API.Enums;
using StudyGO.API.Extensions;
using StudyGO.Contracts.Dtos.TutorProfiles;
using StudyGO.Core.Abstractions.Services.Account;

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
            var result = await _tutorAccountService.TryRegistry(registryRequest, cancellationToken);

            return result.ToActionResult();
        }

        [HttpGet("get-all-profiles")]
        [Authorize]
        public async Task<ActionResult<List<TutorProfileDto>>> GetAllProfiles(
            CancellationToken cancellationToken
        )
        {
            var result = await _tutorAccountService.GetAllUserProfiles(cancellationToken);

            return result.ToActionResult();
        }

        [HttpGet("get-profile-by-id/{userId}")]
        [Authorize(Policy = PolicyNames.UserOrAdmin)]
        public async Task<ActionResult<TutorProfileDto?>> GetProfileById(
            Guid userId,
            CancellationToken cancellationToken
        )
        {
            var result = await _tutorAccountService.TryGetUserProfileById(
                userId,
                cancellationToken
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
                return BadRequest(userId.ErrorMessage);

            var result = await _tutorAccountService.TryGetUserProfileById(
                userId.Value,
                cancellationToken
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
                return Forbid();

            var result = await _tutorAccountService.TryUpdateUserProfile(
                userProfile,
                cancellationToken
            );

            return result.ToActionResult();
        }
    }
}
