using Microsoft.Extensions.Logging;
using StudyGO.Contracts.Result;
using StudyGO.Contracts.Result.ErrorTypes;
using StudyGO.Core.Abstractions.Auth;
using StudyGO.Core.Abstractions.EmailServices;
using StudyGO.Core.Abstractions.Repositories;
using StudyGO.Core.Abstractions.Verification;
using StudyGO.Core.Extensions;

namespace StudyGO.infrastructure.Verification;

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
        _logger.LogDebug("��������� ������...");
        
        string token = _emailTokenProvider.GenerateToken(userId);
        
        _logger.LogDebug("�������� ������...");

        string linkText = "�����";
        
        string verificationLink = $"{endPoint}?userId={userId}&token={token}";
        
        string htmlBody = $"<html><body><p>����� ����������� ���� email, ����������, <a href=\"{verificationLink}\">{linkText}</a>.</p></body></html>";
        
        var result = await _emailService.SendVerificationEmailAsync(email, 
            htmlBody, 
            "������������� �������� � StudyGO", cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogError("��������� � �������������� �� ���� ����������: {Error}", result.ErrorMessage);
            var deleteResult = await _userRepository.Delete(userId, cancellationToken);

            if (!deleteResult.IsSuccess)
            {
                _logger.LogError("������ � ���������������� ������������ � id {userId} �� ���� �������: {Error}", 
                    userId, deleteResult.ErrorMessage);
            }

            return Result<string>.Failure(result.ErrorMessage ?? "", result.ErrorType);
        }
        
        _logger.LogInformation("��������� � �������������� ������������ {userId}, ���� ���������� �� {email}", userId,
            LoggingExtensions.MaskEmail(email));

        var resultInsertToken = await _userRepository.InsertVerifiedToken(userId, token, cancellationToken);

        if (!resultInsertToken.IsSuccess)
            return Result<string>.Failure("������ �����������", ErrorTypeEnum.ServerError);

        return Result<string>.Success(token);
    }
}