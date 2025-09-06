using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using StudyGO.API.Enums;
using StudyGO.API.Extensions;
using StudyGO.API.Options;
using StudyGO.Application.UseCases.TutorProfileUseCases.Commands.RegistryTutor;
using StudyGO.Application.UseCases.TutorProfileUseCases.Commands.UpdateTutor;
using StudyGO.Application.UseCases.TutorProfileUseCases.Queries.GetAll.GetAllTutors;
using StudyGO.Application.UseCases.TutorProfileUseCases.Queries.GetAll.GetAllVerifiedTutors;
using StudyGO.Application.UseCases.TutorProfileUseCases.Queries.GetById.GetTutorById;
using StudyGO.Application.UseCases.TutorProfileUseCases.Queries.GetById.GetVerifiedTutorById;
using StudyGO.Contracts.Contracts;
using StudyGO.Contracts.Dtos.TutorProfiles;
using StudyGO.Contracts.PaginationContract;
using StudyGO.Core.Extensions;

namespace StudyGO.API.Controllers.UsersControllers
{
    [ApiController]
    [Route("[controller]")]
    public class TutorProfileController : ControllerBase
    {
        private readonly ILogger<TutorProfileController> _logger;
        
        private readonly EmailConfirmationOptions _emailOptions;

        private readonly IMediator _mediator;
        
        public TutorProfileController(
            ILogger<TutorProfileController> logger,
            IOptions<EmailConfirmationOptions> emailOptions, 
            IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
            _emailOptions = emailOptions.Value;
        }

        [HttpPost("registry")]
        public async Task<ActionResult<UserRegistryResponse>> RegistryUser(
            [FromBody] TutorProfileRegistrDto registryRequest,
            CancellationToken cancellationToken
        )
        {
            _logger.LogInformation("Попытка регистрации учителя с email: {Email}", 
                LoggingExtensions.MaskEmail(registryRequest.User.Email));
            
            string? confirmEmailEndpoint = Url.Action(
                _emailOptions.Action,
                _emailOptions.Controller,
                null,
                Request.Scheme,
                Request.Host.ToString()
            );

            if (string.IsNullOrWhiteSpace(confirmEmailEndpoint))
            {
                _logger.LogError("Ссылка на контроллер с подтверждением email не была сформирована");
                return new ObjectResult(null) {StatusCode = StatusCodes.Status500InternalServerError};
            }
            
            var result = 
                await _mediator.Send(
                    new RegistryTutorCommand(registryRequest, confirmEmailEndpoint),
                cancellationToken
                    );
            
            _logger.LogResult(result, 
                "Успешная регистрация учителя", 
                "Ошибка регистрации учителя",
                new { UserId = result.Value });
            
            return result.ToActionResult();
        }
        
        [HttpGet("get-all-verified-profiles")]
        [Authorize]
        public async Task<ActionResult<List<TutorProfileDto>>> GetAllProfilesVerified(
            [FromQuery] Pagination paginationParams,
            CancellationToken cancellationToken
        )
        {
            _logger.LogInformation("Запрос всех учителей");
            
            var result = await 
                _mediator.Send(new GetAllVerifiedTutorQuery(paginationParams), cancellationToken);
            
            _logger.LogResult(
                result,
                "Учителя успешно получены",
                "Ошибка при получении списка учителей",
                new { CountTeacher = result.Value?.Count }
            );
            
            return result.ToActionResult();
        }
        
        [HttpGet("get-all-profiles")]
        [Authorize(Policy = PolicyNames.AdminOnly)]
        public async Task<ActionResult<List<TutorProfileDto>>> GetAllProfiles(
            [FromQuery] Pagination paginationParams,
            CancellationToken cancellationToken
        )
        {
            _logger.LogInformation("Запрос всех учителей");
            
            var result = await 
                _mediator.Send(new GetAllTutorQuery(paginationParams), cancellationToken);
            
            _logger.LogResult(
                result,
                "Учителя успешно получены",
                "Ошибка при получении списка учителей",
                new { CountTeacher = result.Value?.Count }
            );
            
            return result.ToActionResult();
        }

        [HttpGet("get-profile-by-id/{userId:guid}")]
        [Authorize(Policy = PolicyNames.AdminOnly)]
        public async Task<ActionResult<TutorProfileDto?>> GetProfileById(
            Guid userId,
            CancellationToken cancellationToken
        )
        {
            _logger.LogInformation("Запрос учителя по ID: {userId}", userId);

            var result = 
                await _mediator.Send(new GetTutorByIdQuery(userId), cancellationToken);
            
            _logger.LogResult(
                result,
                "Учитель найден",
                "Учитель не найден",
                new { UserId = userId }
            );
            
            return result.ToActionResult();
        }
        
        [HttpGet("get-verified-profile-by-id/{userId:guid}")]
        [Authorize(Policy = PolicyNames.UserOrAdmin)]
        public async Task<ActionResult<TutorProfileDto?>> GetProfileByIdVerified(
            Guid userId,
            CancellationToken cancellationToken
        )
        {
            _logger.LogInformation("Запрос учителя по ID: {userId}", userId);

            var result = 
                await _mediator.Send(new GetVerifiedTutorByIdQuery(userId), cancellationToken);
            
            _logger.LogResult(
                result,
                "Учитель найден",
                "Учитель не найден",
                new { UserId = userId }
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
            {
                _logger.LogWarning("Невалидный ID пользователя в токене");
                return BadRequest(userId.ErrorMessage);
            }
            
            _logger.LogDebug("Запрос данных текущего учителя {userId}", userId.Value);

            var result = 
                await _mediator.Send(new GetTutorByIdQuery(userId.Value), cancellationToken);
            
            _logger.LogResult(
                result,
                "Данные учителя успешно получены",
                "Учитель не найден",
                new { UserId = userId }
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
            {
                _logger.LogWarning("Попытка обновления не своего аккаунта: {UserId}", userProfile.UserId);
                return Forbid();
            }
            
            _logger.LogInformation("Обновление учителя {UserId}", userProfile.UserId);

            var result = 
                await _mediator.Send(new UpdateTutorCommand(userProfile), cancellationToken);
            
            _logger.LogResult(
                result,
                "Учитель успешно обновлён",
                "Ошибка обновления учителя",
                new { userProfile.UserId }
            );
            
            return result.ToActionResult();
        }
    }
}
