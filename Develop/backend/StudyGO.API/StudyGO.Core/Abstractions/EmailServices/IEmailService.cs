using StudyGO.Contracts.Result;

namespace StudyGO.Core.Abstractions.EmailServices;

public interface IEmailService
{
    Task<Result<SmtpSendRequest>> SendVerificationEmailAsync(string email, string message, string subject);
}