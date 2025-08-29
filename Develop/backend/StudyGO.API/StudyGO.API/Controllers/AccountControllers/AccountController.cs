using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using StudyGO.API.CustomAttributes;
using StudyGO.API.Enums;
using StudyGO.API.Extensions;
using StudyGO.API.Options;
using StudyGO.Application.UseCases.UserUseCases.Commands.DeleteCommands.DeleteAccount;
using StudyGO.Application.UseCases.UserUseCases.Commands.SpecificCommands.ConfirmEmail;
using StudyGO.Application.UseCases.UserUseCases.Commands.SpecificCommands.LogInUser;
using StudyGO.Application.UseCases.UserUseCases.Commands.UpdateCommands.UpdateUser;
using StudyGO.Application.UseCases.UserUseCases.Commands.UpdateCommands.UpdateUser�redentials;
using StudyGO.Application.UseCases.UserUseCases.Queries.GetAll.GetAllAccount;
using StudyGO.Application.UseCases.UserUseCases.Queries.GetById.GetAccountById;
using StudyGO.Contracts.Contracts;
using StudyGO.Contracts.Dtos.Users;
using StudyGO.Core.Abstractions.Services.Account;
using StudyGO.Core.Extensions;

namespace StudyGO.API.Controllers.AccountControllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly ILogger<AccountController> _logger;

        private readonly IUserAccountService _userAccountService;
        
        private readonly IWebHostEnvironment _env;
        
        private readonly EmailConfirmationOptions _emailOptions;
        
        private readonly IMediator _mediator;
        
        public AccountController(
            ILogger<AccountController> logger,
            IUserAccountService userAccountService, 
            IWebHostEnvironment env,
            IOptions<EmailConfirmationOptions> emailOptions, IMediator mediator)
        {
            _logger = logger;
            _userAccountService = userAccountService;
            _env = env;
            _mediator = mediator;
            _emailOptions = emailOptions.Value;
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserLoginResponseDto>> LoginUser(
            [FromBody] UserLoginRequest loginRequest,
            CancellationToken cancellationToken
        )
        {
            _logger.LogInformation(
                "������� �����. Email: {Email}",
                LoggingExtensions.MaskEmail(loginRequest.Email)
            );

            var result = 
                await _mediator.Send(new LogInUserCommand(loginRequest), cancellationToken);

            _logger.LogResult(
                result,
                "�������� ����",
                "������ �����",
                new
                {
                    Email = LoggingExtensions.MaskEmail(loginRequest.Email),
                    UserId = result.Value?.Id,
                }
            );

            return result.ToActionResult();
        }

        [HttpPost("ConfirmEmail")]
        public async Task<ActionResult<Guid>> ConfirmEmail(
            [FromBody] ConfirmEmailRequest request, 
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new ConfirmEmailCommand(request), cancellationToken);

            return result.ToActionResult();
        }
        
        [HttpGet("ConfirmEmailPage")]
        public async Task<IActionResult> ConfirmEmailPage([FromQuery] Guid userId, [FromQuery] string token)
        {
            var filePath = Path.Combine(_env.WebRootPath, "html", "confirm-email.html");
            
            if (!System.IO.File.Exists(filePath))
            {
                _logger.LogError("���� confirm-email.html �� ������ �� ���� {Path}", filePath);
                return NotFound("�������� ������������� �� �������");
            }
            
            string? confirmEmailEndpoint = Url.Action(
                _emailOptions.ConfirmAction,
                _emailOptions.Controller,
                null,
                Request.Scheme,
                Request.Host.ToString()
            );
            
            var html = await System.IO.File.ReadAllTextAsync(filePath);
            
            html = html.Replace("{USER_ID}", userId.ToString())
                .Replace("{TOKEN}", token).Replace("{ENDPOINT}", confirmEmailEndpoint);

            return Content(html, "text/html");
        }
        
        
        [HttpDelete("delete/{userId:guid}")]
        [Authorize(Policy = PolicyNames.AdminOnly)]
        public async Task<ActionResult<Guid>> DeleteUser(
            Guid userId,
            CancellationToken cancellationToken
        )
        {
            _logger.LogInformation("����� �������� �������� ������������ {UserId}", userId);

            var result = await _mediator.Send(new DeleteAccountCommand(userId), cancellationToken);

            _logger.LogResult(
                result,
                "������������ ������� �����",
                "������ �������� ������������",
                new { UserId = userId }
            );

            return result.ToActionResult();
        }

        [HttpDelete("delete-current-user")]
        [Authorize]
        public async Task<ActionResult<Guid>> DeleteCurrentUser(CancellationToken cancellationToken)
        {
            var userId = User.ExtractGuid();

            if (!userId.IsSuccess)
            {
                _logger.LogWarning("�� ������� �������� ID �������� ������������");
                return BadRequest(userId.ErrorMessage);
            }

            _logger.LogInformation("������������ {userId} �������� �������� ������ ��������", userId.Value);

            var result = await _mediator.Send(new DeleteAccountCommand(userId.Value), cancellationToken);

            _logger.LogResult(
                result,
                "������������ ������� �����",
                "������ �������� ������������",
                new { UserId = userId }
            );

            return result.ToActionResult();
        }

        [HttpGet("all-users")]
        [Authorize(Policy = PolicyNames.AdminOnly)]
        public async Task<ActionResult<List<UserDto>>> GetAllUsers(
            CancellationToken cancellationToken
        )
        {
            _logger.LogInformation("������ ���� �������������");

            var result = await _mediator.Send(new GetAllAccountQuery(), cancellationToken);

            _logger.LogResult(
                result,
                "������������ ������� ��������",
                "������ ��� ��������� ������ �������������",
                new { CountUser = result.Value?.Count }
            );

            return result.ToActionResult();
        }

        [HttpGet("user-by-id/{userId}")]
        [Authorize(Policy = PolicyNames.AdminOnly)]
        public async Task<ActionResult<UserDto?>> GetUserById(
            Guid userId,
            CancellationToken cancellationToken
        )
        {
            _logger.LogInformation("������ ������������ �� ID: {userId}", userId);

            var result = await _mediator.Send( new GetAccountByIdQuery(userId), cancellationToken);

            _logger.LogResult(
                result,
                "������������ ������",
                "������������ �� ������",
                new { UserId = userId }
            );

            return result.ToActionResult();
        }

        [HttpGet("current-user")]
        [Authorize]
        public async Task<ActionResult<UserDto?>> GetCurrentUser(
            CancellationToken cancellationToken
        )
        {
            var userId = User.ExtractGuid();

            if (!userId.IsSuccess)
            {
                _logger.LogWarning("���������� ID ������������ � ������");
                return BadRequest(userId.ErrorMessage);
            }

            _logger.LogDebug("������ ������ �������� ������������ {userId}", userId.Value);

            var result = await _mediator.Send( new GetAccountByIdQuery(userId.Value), cancellationToken);

            _logger.LogResult(
                result,
                "������ ������������ ������� ��������",
                "������������ �� ������",
                new { UserId = userId }
            );

            return result.ToActionResult();
        }

        [HttpPut("update-user")]
        [Authorize]
        public async Task<ActionResult<Guid>> UpdateUser(
            [FromBody] UserUpdateDto updateDto,
            CancellationToken cancellationToken
        )
        {
            if (!User.VerifyGuid(updateDto.UserId))
            {
                _logger.LogWarning("������� ���������� �� ������ ��������: {UserId}", updateDto.UserId);
                return Forbid();
            }

            _logger.LogInformation("���������� ������������ {UserId}", updateDto.UserId);

            var result = await _mediator.Send(new UpdateUserCommand(updateDto), cancellationToken);

            _logger.LogResult(
                result,
                "������������ ������� �������",
                "������ ���������� ������������",
                new { updateDto.UserId }
            );

            return result.ToActionResult();
        }

        [HttpPut("update-user-credentials")]
        [Obsolete("� ������ ������ ���� ����� �������� �����������")]
        [Disabled("����� ������ � ����� ����������")]
        [Authorize]
        public async Task<ActionResult<Guid>> UpdateCredentials(
            [FromBody] UserUpdate�redentialsDto updateDto,
            CancellationToken cancellationToken
        )
        {
            if (!User.VerifyGuid(updateDto.UserId))
            {
                _logger.LogWarning("������� ���������� �� ������ ��������: {UserId}", updateDto.UserId);
                return Forbid();
            }

            _logger.LogInformation("���������� ������� ������ ������������ {UserId}", updateDto.UserId);

            var result = 
                await _mediator.Send(new UpdateUser�redentialsCommand(updateDto), cancellationToken);

            _logger.LogResult(
                result,
                "������� ������ ������������ ������� ���������",
                "������ ���������� ������� ������",
                new { updateDto.UserId }
            );

            return result.ToActionResult();
        }
    }
}
