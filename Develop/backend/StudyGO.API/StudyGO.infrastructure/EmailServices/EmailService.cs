using System.Diagnostics;
using System.Net.Sockets;
using System.Security.Authentication;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
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

    public async Task<SmtpSendRequest> SendEmailAsync(
        string email,
        string message,
        string subject,
        CancellationToken cancellationToken = default
    )
    {
        var sw = Stopwatch.StartNew();
        try
        {
            if (string.IsNullOrWhiteSpace(email))
                return Failure(sw, ErrorSendEmailType.InvalidEmailFormat);

            using var emailMessage = BuildMessage(email, subject, message);
            using var client = new SmtpClient();

            var connectResult = await TryConnectAsync(client, cancellationToken);
            if (connectResult is not null) return Failure(sw, connectResult.Value);

            var authResult = await TryAuthenticateAsync(client, cancellationToken);
            if (authResult is not null) return Failure(sw, authResult.Value);

            var sendResult = await TrySendAsync(client, emailMessage, cancellationToken);
            if (sendResult is not null) return Failure(sw, sendResult.Value);

            await client.DisconnectAsync(true, cancellationToken);
            return Success(sw);
        }
        catch (Exception ex)
        {
            return HandleUnexpectedError(sw, ex);
        }
        finally
        {
            sw.Stop();
        }
    }
    
    private MimeMessage BuildMessage(string email, string subject, string body)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_options.Fromname, _options.Username));
        message.To.Add(new MailboxAddress("", email));
        message.Subject = subject;
        message.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = body };
        return message;
    }
    
    private async Task<ErrorSendEmailType?> TryConnectAsync(SmtpClient client, CancellationToken token)
    {
        try
        {
            await client.ConnectAsync(_options.SmtpServer, _options.Port, true, token);
            return null;
        }
        catch (SmtpCommandException ex) when (ex.StatusCode == SmtpStatusCode.ServiceNotAvailable)
        {
            _logger.LogError("SMTP сервер недоступен: {Error}", ex.Message);
            return ErrorSendEmailType.SmtpServerUnavailable;
        }
        catch (SocketException ex)
        {
            _logger.LogError("ќшибка сети: {Error}", ex.Message);
            return ErrorSendEmailType.NetworkError;
        }
    }
    
    private async Task<ErrorSendEmailType?> TryAuthenticateAsync(SmtpClient client, CancellationToken token)
    {
        try
        {
            await client.AuthenticateAsync(_options.Username, _options.Password, token);
            return null;
        }
        catch (AuthenticationException ex)
        {
            _logger.LogError("ќшибка аутентификации: {Error}", ex.Message);
            return ErrorSendEmailType.SmtpAuthenticationFailed;
        }
    }
    
    private async Task<ErrorSendEmailType?> TrySendAsync(SmtpClient client, MimeMessage message, CancellationToken token)
    {
        try
        {
            await client.SendAsync(message, token);
            return null;
        }
        catch (SmtpCommandException ex) when (ex.StatusCode == SmtpStatusCode.MailboxUnavailable)
        {
            _logger.LogInformation("ящик не существует: {Error}", ex.Message);
            return ErrorSendEmailType.MailboxDoesNotExist;
        }
        catch (SmtpCommandException ex) when (ex.StatusCode == SmtpStatusCode.ExceededStorageAllocation)
        {
            _logger.LogError("ящик переполнен: {Error}", ex.Message);
            return ErrorSendEmailType.MailboxFull;
        }
    }

    private SmtpSendRequest Failure(Stopwatch sw, ErrorSendEmailType type) =>
        SmtpSendRequest.FailureSend(sw.Elapsed, type);

    private SmtpSendRequest Success(Stopwatch sw) =>
        SmtpSendRequest.SuccessSend(sw.Elapsed);

    private SmtpSendRequest HandleUnexpectedError(Stopwatch sw, Exception ex)
    {
        return ex switch
        {
            TimeoutException => Failure(sw, ErrorSendEmailType.OperationTimedOut),
            ArgumentException => Failure(sw, ErrorSendEmailType.InvalidEmailFormat),
            _ => LogAndReturnServerError(sw, ex)
        };
    }
    
    private SmtpSendRequest LogAndReturnServerError(Stopwatch sw, Exception ex)
    {
        _logger.LogError(ex, "Ќеизвестна€ ошибка при отправке email");
        return Failure(sw, ErrorSendEmailType.ServerError);
    }
}
