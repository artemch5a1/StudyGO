using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyGO.API.Enums;
using StudyGO.API.Extensions;
using StudyGO.Contracts.Dtos.UserProfiles;
using StudyGO.Contracts.PaginationContract;
using StudyGO.Core.Abstractions.Services.Account;
using StudyGO.Core.Extensions;

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
            _logger.LogInformation("������� ����������� ������������ � email: {Email}", 
                LoggingExtensions.MaskEmail(registryRequest.User.Email));
            
            string baseUrl = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";
            
            string confirmEmailEndpoint = $"{baseUrl}/Account/ConfirmEmailPage";
            
            var result = await _userAccountService.TryRegistry(registryRequest, confirmEmailEndpoint, cancellationToken);
            
            _logger.LogResult(result, 
                "�������� ����������� ������������", 
                "������ ����������� ������������",
                new { UserId = result.Value });
            
            return result.ToActionResult();
        }

        [HttpGet("get-all-verified-profiles")]
        [Authorize(Policy = PolicyNames.AdminOnly)]
        public async Task<ActionResult<List<UserProfileDto>>> GetAllVerifiedProfiles(
            [FromQuery] Pagination paginationParams,
            CancellationToken cancellationToken
        )
        {
            _logger.LogInformation("������ ���� �������������� �������� �������������");
            
            var result = await _userAccountService.GetAllUserVerifiedProfiles(cancellationToken, paginationParams);
            
            _logger.LogResult(
                result,
                "������� ������� ��������",
                "������ ��� ��������� ������ �������������� ��������",
                new { CountTeacher = result.Value?.Count }
            );
            
            return result.ToActionResult();
        }
        
        [HttpGet("get-all-profiles")]
        [Authorize(Policy = PolicyNames.AdminOnly)]
        public async Task<ActionResult<List<UserProfileDto>>> GetAllProfiles(
            [FromQuery] Pagination paginationParams,
            CancellationToken cancellationToken
        )
        {
            _logger.LogInformation("������ ���� �������� �������������");
            
            var result = await _userAccountService.GetAllUserProfiles(cancellationToken, paginationParams);
            
            _logger.LogResult(
                result,
                "������� ������� ��������",
                "������ ��� ��������� ������ ��������",
                new { CountTeacher = result.Value?.Count }
            );
            
            return result.ToActionResult();
        }
        
        [HttpGet("get-profile-by-id/{userId:guid}")]
        [Authorize(Policy = PolicyNames.AdminOnly)]
        public async Task<ActionResult<UserProfileDto?>> GetProfileById(
            Guid userId,
            CancellationToken cancellationToken
        )
        {
            _logger.LogInformation("������ ������� ������������ �� ID: {userId}", userId);
            
            var result = await _userAccountService.TryGetUserProfileById(userId, cancellationToken);
            
            _logger.LogResult(
                result,
                "������� ������",
                "������� �� ������",
                new { UserId = userId }
            );
            
            return result.ToActionResult();
        }
        
        [HttpGet("get-verified-profile-by-id/{userId:guid}")]
        [Authorize(Policy = PolicyNames.AdminOnly)]
        public async Task<ActionResult<UserProfileDto?>> GetVerifiedProfileById(
            Guid userId,
            CancellationToken cancellationToken
        )
        {
            _logger.LogInformation("������ ������� ������������ �� ID: {userId}", userId);
            
            var result = await _userAccountService.TryGetVerifiedUserProfileById(userId, cancellationToken);
            
            _logger.LogResult(
                result,
                "������� ������",
                "������� �� ������ ����� ��������������",
                new { UserId = userId }
            );
            
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
            {
                _logger.LogWarning("���������� ID ������������ � ������");
                return BadRequest(userId.ErrorMessage);
            }
            
            _logger.LogDebug("������ ������ �������� ������� ������������ {UserId}", userId.Value);
            
            var result = await _userAccountService.TryGetUserProfileById(
                userId.Value,
                cancellationToken
            );
            
            _logger.LogResult(
                result,
                "������ ������� ������� ��������",
                "������� �� ������",
                new { UserId = userId }
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
            {
                _logger.LogWarning("������� ���������� �� ������ �������: {UserId}", userProfile.UserId);
                return Forbid();
            }

            var result = await _userAccountService.TryUpdateUserProfile(
                userProfile,
                cancellationToken
            );
            
            _logger.LogResult(
                result,
                "������� ������� �������",
                "������ ���������� �������",
                new { userProfile.UserId }
            );
            
            return result.ToActionResult();
        }
    }
}
