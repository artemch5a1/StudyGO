using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyGO.API.Enums;
using StudyGO.Contracts.Dtos.TutorProfiles;
using StudyGO.Core.Abstractions.Services.Account;
using StudyGO.API.Extensions;

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

        [HttpPost("registr")]
        public async Task<ActionResult<Guid>> RegistrUser(
            [FromBody] TutorProfileRegistrDto registrRequest, CancellationToken cancellationToken
        )
        {
            var result = await _tutorAccountService.TryRegistr(registrRequest, cancellationToken);

            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.ErrorMessage);
        }

        [HttpGet("get-all-profiles")]
        [Authorize]
        public async Task<ActionResult<List<TutorProfileDto>>> GetAllProfiles(CancellationToken cancellationToken)
        {
            var result = await _tutorAccountService.GetAllUserProfiles(cancellationToken);

            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.ErrorMessage);
        }

        [HttpGet("get-profile-by-id/{userID}")]
        [Authorize(Policy = PolicyNames.UserOrAdmin)]
        public async Task<ActionResult<TutorProfileDto>> GetProfileById(Guid userID, CancellationToken cancellationToken)
        {
            var result = await _tutorAccountService.TryGetUserProfileById(userID, cancellationToken);

            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.ErrorMessage);
        }

        [HttpGet("get-current-profile")]
        [Authorize]
        public async Task<ActionResult<TutorProfileDto>> GetCurrentUser(CancellationToken cancellationToken)
        {
            var userId = User.ExtractGuid();

            if (!userId.IsSuccess)
                return BadRequest(userId.ErrorMessage);

            var result = await _tutorAccountService.TryGetUserProfileById(userId.Value, cancellationToken);

            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.ErrorMessage);
        }


        [HttpPut("update-profile")]
        [Authorize(Policy = PolicyNames.TutorOnly)]
        public async Task<ActionResult<Guid>> UpdateProfile(
            [FromBody] TutorProfileUpdateDto userProfile, CancellationToken cancellationToken
        )
        {
            if (!User.VerifyGuid(userProfile.UserID))
                return Forbid();

            var result = await _tutorAccountService.TryUpdateUserProfile(userProfile, cancellationToken);

            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.ErrorMessage);
        }
    }
}
