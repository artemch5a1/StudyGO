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

    public async Task<SmtpSendRequest> SendVerificationEmailAsync(
        string email,
        string message,
        string subject,
        CancellationToken cancellationToken = default
    )
    {
        var sw = Stopwatch.StartNew();
        try
        {
            if(string.IsNullOrWhiteSpace(email))
            {
                return SmtpSendRequest
                    .FailureSend(sw.Elapsed, ErrorSendEmailType.InvalidEmailFormat);
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
                _logger
                    .LogError("Ошибка при отправки сообщения через {ServiceName}: {Error}"
                        , nameof(EmailService), ex.Message);
                return SmtpSendRequest
                    .FailureSend(sw.Elapsed, ErrorSendEmailType.SmtpServerUnavailable);
            }
            catch (SocketException ex)
            {
                _logger
                    .LogError("Ошибка при отправки сообщения через {ServiceName}: {Error}"
                        , nameof(EmailService), ex.Message);
                return SmtpSendRequest
                    .FailureSend(sw.Elapsed, ErrorSendEmailType.NetworkError);
            }

            try
            {
                await client.AuthenticateAsync(_options.Username, _options.Password, cancellationToken);
            }
            catch (AuthenticationException ex)
            {
                _logger
                    .LogError("Ошибка при отправки сообщения через {ServiceName}: {Error}"
                        , nameof(EmailService), ex.Message);
                return SmtpSendRequest
                    .FailureSend(sw.Elapsed, ErrorSendEmailType.SmtpAuthenticationFailed);
            }

            try
            {
                await client.SendAsync(emailMessage, cancellationToken);
            }
            catch (SmtpCommandException ex)
                when (ex.StatusCode == SmtpStatusCode.MailboxUnavailable)
            {
                _logger
                    .LogInformation("Ошибка при отправки сообщения через {ServiceName}: {Error}"
                        , nameof(EmailService), ex.Message);
                return SmtpSendRequest
                    .FailureSend(sw.Elapsed, ErrorSendEmailType.MailboxDoesNotExist);
            }
            catch (SmtpCommandException ex)
                when (ex.StatusCode == SmtpStatusCode.ExceededStorageAllocation)
            {
                _logger
                    .LogError("Ошибка при отправки сообщения через {ServiceName}: {Error}"
                        , nameof(EmailService), ex.Message);
                return SmtpSendRequest
                    .FailureSend(sw.Elapsed, ErrorSendEmailType.MailboxFull);
            }

            await client.DisconnectAsync(true, cancellationToken);
            return SmtpSendRequest
                .SuccessSend(sw.Elapsed);
        }
        catch (TimeoutException ex)
        {
            _logger
                .LogError("Ошибка при отправки сообщения через {ServiceName}: {Error}"
                    , nameof(EmailService), ex.Message);
            return SmtpSendRequest
                .FailureSend(sw.Elapsed, ErrorSendEmailType.OperationTimedOut);
        }
        catch (ArgumentException ex)
        {
            _logger
                .LogInformation("Ошибка при отправки сообщения через {ServiceName}: {Error}"
                    , nameof(EmailService), ex.Message);
            return SmtpSendRequest
                .FailureSend(sw.Elapsed, ErrorSendEmailType.InvalidEmailFormat);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Неизвестная ошибка при отправке email");
            return SmtpSendRequest
                .FailureSend(sw.Elapsed, ErrorSendEmailType.ServerError);
        }
        finally
        {
            sw.Stop();
        }
    }
}
