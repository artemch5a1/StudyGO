using Microsoft.Extensions.Logging;
using StudyGO.Contracts.Result;
using StudyGO.Contracts.Result.ErrorTypes;
using StudyGO.Core.Abstractions.Auth;
using StudyGO.Core.Abstractions.EmailServices;
using StudyGO.Core.Abstractions.Repositories;
using StudyGO.Core.Abstractions.Verification;
using StudyGO.Core.Extensions;

namespace StudyGO.infrastructure.Extensions;

public class VerificationService : IVerificationService
{
    private readonly IEmailService _emailService;
    
    private readonly IEmailVerifyTokenProvider _emailTokenProvider;

    private readonly IUserRepository _userRepository;

    private readonly ILogger<VerificationService> _logger;
    
    public VerificationService(
        IEmailService emailService,
        IEmailVerifyTokenProvider emailTokenProvider,
        IUserRepository userRepository, ILogger<VerificationService> logger)
    {
        _emailService = emailService;
        _emailTokenProvider = emailTokenProvider;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<Result<string>> CreateTokenAndSendMessage(Guid userId, 
        string email, 
        string endPoint, 
        CancellationToken cancellationToken)
    {
        _logger.LogDebug("Генерация токена...");
        
        string token = _emailTokenProvider.GenerateToken(userId);
        
        _logger.LogDebug("Создание ссылки...");

        string htmlBody = BuildVerificationEmailBody(userId, token, endPoint);
        
        
        var result = await SendVerificationEmail(email, htmlBody, cancellationToken);

        if (!result.Success)
        {
            return await HandleEmailFailure(userId, result, cancellationToken);
        }
        
        _logger.LogInformation("Сообщение с подтверждением пользователя {userId}, была отправлена на {email}", userId,
            LoggingExtensions.MaskEmail(email));

        var resultInsertToken = await SaveToken(userId, token, cancellationToken);

        if (!resultInsertToken.IsSuccess)
            return Result<string>.Failure("Ошибка регистрации", ErrorTypeEnum.ServerError);

        return Result<string>.Success(token);
    }
    
    private string BuildVerificationEmailBody(Guid userId, string token, string endPoint)
    {
        _logger.LogDebug("Создание ссылки...");
        string linkText = "сюда";
        string verificationLink = $"{endPoint}?userId={userId}&token={token}";
        return $"<html><body><p>Чтобы подтвердить свой email, пожалуйста, перейдите " +
               $"<a href=\"{verificationLink}\">{linkText}</a>.</p></body></html>";
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

        var deleteResult = await _userRepository.Delete(userId, cancellationToken);
        if (!deleteResult.IsSuccess)
        {
            _logger.LogError("Запись о неподтвержденном пользователе с id {userId} не была удалена: {Error}", 
                userId, deleteResult.ErrorMessage);
        }

        return resultFailure;
    }
    
    private Task<Result<Guid>> SaveToken(Guid userId, string token, CancellationToken cancellationToken)
    {
        return _userRepository.InsertVerifiedToken(userId, token, cancellationToken);
    }
}