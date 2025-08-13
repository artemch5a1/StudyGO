using StudyGO.Contracts.Result;
using StudyGO.Core.Enums;

namespace StudyGO.Core.Abstractions.EmailServices;

public interface IEmailService
{
    Task<ResultError<SmtpSendRequest, ErrorSendEmailType>> SendVerificationEmailAsync(string email, string message, string subject);
}