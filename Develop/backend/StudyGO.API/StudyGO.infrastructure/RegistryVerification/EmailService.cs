using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using StudyGO.Contracts.Result;
using StudyGO.Contracts.Result.ErrorTypes;
using StudyGO.Core.Abstractions.EmailServices;

namespace StudyGO.infrastructure.RegistryVerification;

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;
    private readonly EmailServiceOptions _options;

    public EmailService(IOptions<EmailServiceOptions> options, ILogger<EmailService> logger)
    {
        _logger = logger;
        _options = options.Value;
    }
    
    public async Task<Result<SmtpSendRequest>> SendVerificationEmailAsync(string email, string message, string subject)
    {
        try
        {
            using var emailMessage = new MimeMessage();
 
            emailMessage.From.Add(new MailboxAddress(_options.Fromname, _options.Username));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = message
            };

            using var client = new SmtpClient();
            await client.ConnectAsync(_options.SmtpServer, _options.Port, true);
            await client.AuthenticateAsync(_options.Username, _options.Password);
            await client.SendAsync(emailMessage);
 
            await client.DisconnectAsync(true);

            return Result<SmtpSendRequest>.SuccessWithoutValue();
        }
        catch (Exception e)
        {
            _logger.LogInformation("Произошла ошибка при отправке сообщения по адресу {Email}: {Message}", email, e.Message);
            return Result<SmtpSendRequest>.Failure(e.Message, ErrorTypeEnum.ServerError);
        }
    }
}