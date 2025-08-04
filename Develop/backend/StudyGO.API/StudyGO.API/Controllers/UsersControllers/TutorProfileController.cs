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
            [FromBody] TutorProfileRegistrDto registrRequest
        )
        {
            var result = await _tutorAccountService.TryRegistr(registrRequest);

            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.ErrorMessage);
        }

        [HttpGet("get-all-profiles")]
        [Authorize]
        public async Task<ActionResult<List<TutorProfileDto>>> GetAllProfiles()
        {
            var result = await _tutorAccountService.GetAllUserProfiles();

            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.ErrorMessage);
        }

        [HttpGet("get-profile-by-id/{userID}")]
        [Authorize(Policy = PolicyNames.UserOrAdmin)]
        public async Task<ActionResult<TutorProfileDto>> GetProfileById(Guid userID)
        {
            var result = await _tutorAccountService.TryGetUserProfileById(userID);

            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.ErrorMessage);
        }

        [HttpGet("get-current-profile")]
        [Authorize]
        public async Task<ActionResult<TutorProfileDto>> GetCurrentUser()
        {
            var userId = User.ExtractGuid();

            if (!userId.IsSuccess)
                return BadRequest(userId.ErrorMessage);

            var result = await _tutorAccountService.TryGetUserProfileById(userId.Value);

            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.ErrorMessage);
        }


        [HttpPut("update-profile")]
        [Authorize(Policy = PolicyNames.TutorOnly)]
        public async Task<ActionResult<Guid>> UpdateProfile(
            [FromBody] TutorProfileUpdateDto userProfile
        )
        {
            if (!User.VerifyGuid(userProfile.UserID))
                return Unauthorized("Доступ запрещен");

            var result = await _tutorAccountService.TryUpdateUserProfile(userProfile);

            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.ErrorMessage);
        }
    }
}
