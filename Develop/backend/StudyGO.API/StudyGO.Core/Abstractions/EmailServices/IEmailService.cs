namespace StudyGO.Core.Abstractions.EmailServices;

public interface IEmailService
{
    Task<SmtpSendRequest> SendEmailAsync(string email, string message, string subject, CancellationToken cancellationToken = default);
}