using StudyGO.Core.Enums;

namespace StudyGO.Core.Abstractions.EmailServices;

public record SmtpSendRequest
{
    private SmtpSendRequest(
        bool success, 
        TimeSpan timeSend, 
        ErrorSendEmailType? errorType = null)
    {
        Success = success;
        TimeSend = timeSend;
        Error = errorType;
    }

    public bool Success { get; set; }

    public TimeSpan TimeSend { get; set; }

    public ErrorSendEmailType? Error { get; set; }

    public static SmtpSendRequest SuccessSend(TimeSpan timeSend)
    {
        return new SmtpSendRequest(true, timeSend);
    }
    
    public static SmtpSendRequest FailureSend(TimeSpan timeSend, ErrorSendEmailType error)
    {
        return new SmtpSendRequest(false, timeSend, error);
    }
}