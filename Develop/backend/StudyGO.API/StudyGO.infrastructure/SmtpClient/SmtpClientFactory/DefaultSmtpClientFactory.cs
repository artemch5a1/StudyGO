using MailKit.Net.Smtp;

namespace StudyGO.infrastructure.SmtpClientFactory.SmtpClient;

public class DefaultSmtpClientFactory : ISmtpClientFactory
{
    public ISmtpClient CreateClient()
    {
        return new MailKit.Net.Smtp.SmtpClient();
    }
}