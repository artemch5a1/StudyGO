using StudyGO.Contracts.Result;
using StudyGO.Core.Enums;

namespace StudyGO.Core.Abstractions.EmailServices;

public interface IEmailService
{
    Task<SmtpSendRequest> SendVerificationEmailAsync(string email, string message, string subject, CancellationToken cancellationToken = default);
}