namespace StudyGO.Core.Abstractions.EmailServices;

public record SmtpSendRequest
{
    public bool Success { get; set; }
}