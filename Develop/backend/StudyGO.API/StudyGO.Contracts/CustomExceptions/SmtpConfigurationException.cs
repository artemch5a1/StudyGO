namespace StudyGO.Contracts.CustomExceptions;

public class SmtpConfigurationException : Exception
{
    public SmtpConfigurationException(string message, Exception ex) : base(message, ex)
    {
        
    }
}