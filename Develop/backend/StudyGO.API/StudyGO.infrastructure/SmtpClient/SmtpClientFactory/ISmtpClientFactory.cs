using MailKit.Net.Smtp;

namespace StudyGO.infrastructure.SmtpClientFactory.SmtpClient;

public interface ISmtpClientFactory
{
    ISmtpClient CreateClient();
}