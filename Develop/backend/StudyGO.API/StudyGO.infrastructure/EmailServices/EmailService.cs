using System.Net.Sockets;
using System.Security.Authentication;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using StudyGO.Contracts.Result;
using StudyGO.Contracts.Result.ErrorTypes;
using StudyGO.Core.Abstractions.EmailServices;
using StudyGO.Core.Enums;

namespace StudyGO.infrastructure.EmailServices;

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;
    private readonly EmailServiceOptions _options;

    public EmailService(IOptions<EmailServiceOptions> options, ILogger<EmailService> logger)
    {
        _logger = logger;
        _options = options.Value;
    }

    public async Task<ResultError<SmtpSendRequest, ErrorSendEmailType>> SendVerificationEmailAsync(
        string email,
        string message,
        string subject,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return ResultError<SmtpSendRequest, ErrorSendEmailType>.Failure(
                    "Email не может быть пустым",
                    ErrorSendEmailType.InvalidEmailFormat,
                    ErrorTypeEnum.ValidationError
                );
            }

            using var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(_options.Fromname, _options.Username));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = message };

            using var client = new SmtpClient();

            try
            {
                await client.ConnectAsync(_options.SmtpServer, _options.Port, true, cancellationToken);
            }
            catch (SmtpCommandException ex)
                when (ex.StatusCode == SmtpStatusCode.ServiceNotAvailable)
            {
                return ResultError<SmtpSendRequest, ErrorSendEmailType>.Failure(
                    ex.Message,
                    ErrorSendEmailType.SmtpServerUnavailable,
                    ErrorTypeEnum.ServerError
                );
            }
            catch (SocketException ex)
            {
                return ResultError<SmtpSendRequest, ErrorSendEmailType>.Failure(
                    ex.Message,
                    ErrorSendEmailType.NetworkError,
                    ErrorTypeEnum.ServerError
                );
            }

            try
            {
                await client.AuthenticateAsync(_options.Username, _options.Password, cancellationToken);
            }
            catch (AuthenticationException ex)
            {
                return ResultError<SmtpSendRequest, ErrorSendEmailType>.Failure(
                    ex.Message,
                    ErrorSendEmailType.SmtpAuthenticationFailed,
                    ErrorTypeEnum.AuthenticationError
                );
            }

            try
            {
                await client.SendAsync(emailMessage, cancellationToken);
            }
            catch (SmtpCommandException ex)
                when (ex.StatusCode == SmtpStatusCode.MailboxUnavailable)
            {
                return ResultError<SmtpSendRequest, ErrorSendEmailType>.Failure(
                    "Email не найден",
                    ErrorSendEmailType.MailboxDoesNotExist,
                    ErrorTypeEnum.ValidationError
                );
            }
            catch (SmtpCommandException ex)
                when (ex.StatusCode == SmtpStatusCode.ExceededStorageAllocation)
            {
                return ResultError<SmtpSendRequest, ErrorSendEmailType>.Failure(
                    ex.Message,
                    ErrorSendEmailType.MailboxFull,
                    ErrorTypeEnum.ValidationError
                );
            }

            await client.DisconnectAsync(true, cancellationToken);
            return ResultError<SmtpSendRequest, ErrorSendEmailType>.SuccessWithoutValue();
        }
        catch (TimeoutException ex)
        {
            return ResultError<SmtpSendRequest, ErrorSendEmailType>.Failure(
                ex.Message,
                ErrorSendEmailType.OperationTimedOut,
                ErrorTypeEnum.ServerError
            );
        }
        catch (ArgumentException ex)
        {
            return ResultError<SmtpSendRequest, ErrorSendEmailType>.Failure(
                ex.Message,
                ErrorSendEmailType.InvalidEmailFormat,
                ErrorTypeEnum.ValidationError
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Неизвестная ошибка при отправке email");
            return ResultError<SmtpSendRequest, ErrorSendEmailType>.Failure(
                ex.Message,
                ErrorSendEmailType.ServerError,
                ErrorTypeEnum.ServerError
            );
        }
    }
}
