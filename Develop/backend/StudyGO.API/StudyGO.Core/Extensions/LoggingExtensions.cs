using Microsoft.Extensions.Logging;
using StudyGO.Contracts.Result;

namespace StudyGO.Core.Extensions;

public static  class LoggingExtensions
{
    public static void LogResult<T>(
        this ILogger logger,
        ResultBase<T> result,
        string successMessage,
        string errorMessage,
        object? context = null
    )
    {
        if (result.IsSuccess)
        {
            logger.LogInformation("{Message} {@Context}", successMessage, context);
        }
        else
        {
            logger.LogWarning("{Message}: {Error} {@Context}", errorMessage, result.ErrorMessage, context);
        }
    }
    
    public static string MaskEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
            return "***";

        var parts = email.Split('@');
        var namePart = parts[0];
        var domainPart = parts[1];

        var maskedName = namePart.Length > 2
            ? namePart.Substring(0, 2) + new string('*', namePart.Length - 2)
            : namePart[0] + "*";

        return $"{maskedName}@{domainPart}";
    }
}