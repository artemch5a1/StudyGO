using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyGO.API.Enums;
using StudyGO.Contracts.Dtos.TutorProfiles;
using StudyGO.Core.Abstractions.Services.Account;
using System.Security.Claims;

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
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
            {
                return Unauthorized("Invalid user ID in token");
            }

            var result = await _tutorAccountService.TryGetUserProfileById(userGuid);

            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.ErrorMessage);
        }


        [HttpPut("update-profile")]
        [Authorize(Policy = PolicyNames.TutorOnly)]
        public async Task<ActionResult<Guid>> UpdateProfile(
            [FromBody] TutorProfileUpdateDto userProfile
        )
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if(string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
            {
                return Unauthorized("Invalid user ID in token");
            }

            if(userGuid != userProfile.UserID)
            {
                return Unauthorized("Попытка обновить другого пользователя");
            }

            var result = await _tutorAccountService.TryUpdateUserProfile(userProfile);

            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.ErrorMessage);
        }
    }
}
