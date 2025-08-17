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

        string linkText = "здесь";
        
        string verificationLink = $"{endPoint}?userId={userId}&token={token}";
        
        string htmlBody = $"<html><body><p>Чтобы подтвердить свой email, пожалуйста, <a href=\"{verificationLink}\">{linkText}</a>.</p></body></html>";
        
        var result = await _emailService.SendVerificationEmailAsync(email, 
            htmlBody, 
            "Подтверждение аккаунта в StudyGO", cancellationToken);

        if (!result.Success)
        {
            var resultFailure = result.ToResultFailure<string>();
            
            _logger.LogError("Сообщение с подтверждением не было отправлено: {Error}", resultFailure.ErrorMessage);
            var deleteResult = await _userRepository.Delete(userId, cancellationToken);

            if (!deleteResult.IsSuccess)
            {
                _logger.LogError("Запись о неподтвержденном пользователе с id {userId} не была удалена: {Error}", 
                    userId, deleteResult.ErrorMessage);
            }

            return resultFailure;
        }
        
        _logger.LogInformation("Сообщение с подтверждением пользователя {userId}, была отправлена на {email}", userId,
            LoggingExtensions.MaskEmail(email));

        var resultInsertToken = await _userRepository.InsertVerifiedToken(userId, token, cancellationToken);

        if (!resultInsertToken.IsSuccess)
            return Result<string>.Failure("Ошибка регистрации", ErrorTypeEnum.ServerError);

        return Result<string>.Success(token);
    }
}