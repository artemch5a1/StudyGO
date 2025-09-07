using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using StudyGO.Contracts.Result;
using StudyGO.Contracts.Result.ErrorTypes;
using StudyGO.Core.Abstractions.Auth;
using StudyGO.Core.Abstractions.EmailServices;
using StudyGO.Core.Abstractions.Repositories;
using StudyGO.Core.Abstractions.Verification;
using StudyGO.Core.Extensions;

namespace StudyGO.infrastructure.Extensions;

public class VerificationByLinkService : IVerificationByLinkService
{
    private readonly IEmailService _emailService;
    
    private readonly IEmailVerifyTokenProvider _emailTokenProvider;

    private readonly IUserRepository _userRepository;

    private readonly ILogger<VerificationByLinkService> _logger;
    
    private readonly IWebHostEnvironment _env;
    
    public VerificationByLinkService(
        IEmailService emailService,
        IEmailVerifyTokenProvider emailTokenProvider,
        IUserRepository userRepository, 
        ILogger<VerificationByLinkService> logger, 
        IWebHostEnvironment env)
    {
        _emailService = emailService;
        _emailTokenProvider = emailTokenProvider;
        _userRepository = userRepository;
        _logger = logger;
        _env = env;
    }

    public async Task<Result<string>> CreateTokenAndSendMessage(Guid userId, 
        string email, 
        string endPoint, 
        CancellationToken cancellationToken)
    {
        _logger.LogDebug("Генерация токена...");
        
        string token = _emailTokenProvider.GenerateToken(userId);
        
        _logger.LogDebug("Создание ссылки...");

        string htmlBody = await BuildVerificationEmailBody(userId, token, endPoint);
        
        
        var result = await SendVerificationEmail(email, htmlBody, cancellationToken);

        if (!result.Success)
        {
            return await HandleEmailFailure(userId, result, cancellationToken);
        }
        
        _logger.LogInformation("Сообщение с подтверждением пользователя {userId}, была отправлена на {email}", userId,
            LoggingExtensions.MaskEmail(email));

        var resultInsertToken = await SaveToken(userId, token, cancellationToken);

        if (!resultInsertToken.IsSuccess)
        {
            return Result<string>.Failure("Ошибка регистрации", ErrorTypeEnum.ServerError);
        }

        return Result<string>.Success(token);
    }
    
    private async Task<string> BuildVerificationEmailBody(Guid userId, string token, string endPoint)
    {
        _logger.LogDebug("Создание ссылки...");
        string verificationLink = $"{endPoint}?userId={userId}&token={token}";
        
        
        var filePath = Path.Combine(_env.WebRootPath, "html", "confirm-email-template-message.html");
        
        var html = await System.IO.File.ReadAllTextAsync(filePath);

        html = html.Replace("{Link}", verificationLink);
        
        return html;
    }
    
    private async Task<SmtpSendRequest> SendVerificationEmail(string email, string body, CancellationToken cancellationToken)
    {
        var result = await _emailService.SendEmailAsync(
            email, 
            body, 
            "Подтверждение аккаунта в StudyGO", 
            cancellationToken);

        if (result.Success)
        {
            _logger.LogInformation(
                "Сообщение с подтверждением отправлено на {email}", 
                LoggingExtensions.MaskEmail(email));
        }

        return result;
    }
    
    private async Task<Result<string>> HandleEmailFailure(Guid userId, SmtpSendRequest sendResult, CancellationToken cancellationToken)
    {
        var resultFailure = sendResult.ToResultFailure<string>();
        _logger.LogError("Сообщение с подтверждением не было отправлено: {Error}", resultFailure.ErrorMessage);
        
        await Task.CompletedTask;
        return resultFailure;
    }
    
    private Task<Result<Guid>> SaveToken(Guid userId, string token, CancellationToken cancellationToken)
    {
        return _userRepository.InsertVerifiedToken(userId, token, cancellationToken);
    }

    public async Task RollBackUser(Guid userId, CancellationToken cancellationToken = default)
    {
        var deleteResult = await _userRepository.Delete(userId, cancellationToken);
        if (!deleteResult.IsSuccess)
        {
            _logger.LogError("Запись о неподтвержденном пользователе с id {userId} не была удалена: {Error}", 
                userId, deleteResult.ErrorMessage);
        }
    }
}