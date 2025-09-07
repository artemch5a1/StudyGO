using System.Diagnostics;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using StudyGO.Core.Abstractions.EmailServices;
using StudyGO.Core.Enums;
using StudyGO.infrastructure.SmtpClient;

namespace StudyGO.infrastructure.EmailServices;

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;
    private readonly ISmtpSender _sender;
    private readonly EmailServiceOptions _options;

    public EmailService(
        IOptions<EmailServiceOptions> options, 
        ILogger<EmailService> logger,
        ISmtpSender sender)
    {
        _logger = logger;
        _sender = sender;
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

            var result = await TrySendAsync(emailMessage, cancellationToken);

            if (result != null)
            {
                return Failure(sw, result ?? ErrorSendEmailType.ServerError);
            }

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
    
    private async Task<ErrorSendEmailType?> TrySendAsync(MimeMessage message, CancellationToken token)
    {
        try
        {
            await _sender.SendAsync(message, token);
            return null;
        }
        catch (SmtpCommandException ex) when (ex.StatusCode == SmtpStatusCode.MailboxUnavailable)
        {
            _logger.LogInformation("Ящик не существует: {Error}", ex.Message);
            return ErrorSendEmailType.MailboxDoesNotExist;
        }
        catch (SmtpCommandException ex) when (ex.StatusCode == SmtpStatusCode.ExceededStorageAllocation)
        {
            _logger.LogError("Ящик переполнен: {Error}", ex.Message);
            return ErrorSendEmailType.MailboxFull;
        }
        catch (SmtpCommandException ex)
        {
            _logger.LogError(ex, "Ошибка SMTP команды");
            return ErrorSendEmailType.SmtpServerUnavailable;
        }
        catch (SmtpProtocolException ex)
        {
            _logger.LogError(ex, "Ошибка протокола SMTP");
            return ErrorSendEmailType.SslHandshakeFailed;
        }
        catch (IOException ex)
        {
            _logger.LogError(ex, "Ошибка ввода/вывода при отправке письма");
            return ErrorSendEmailType.SmtpServerUnavailable;
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Непредвиденная ошибка при отправке письма");
            throw;
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
        _logger.LogError(ex, "Неизвестная ошибка при отправке email");
        return Failure(sw, ErrorSendEmailType.ServerError);
    }
}
