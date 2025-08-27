using MimeKit;

namespace StudyGO.infrastructure.SmtpClient;

public interface ISmtpSender
{
    Task SendAsync(MimeMessage message, CancellationToken ct = default);
}