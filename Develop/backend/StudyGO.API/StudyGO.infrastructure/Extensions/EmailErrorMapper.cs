using System.ComponentModel;
using System.Reflection;
using StudyGO.Contracts.Result;
using StudyGO.Contracts.Result.ErrorTypes;
using StudyGO.Core.Abstractions.EmailServices;
using StudyGO.Core.Enums;

namespace StudyGO.infrastructure.Extensions;

public static class EmailErrorMapper
{
    private static readonly Dictionary<ErrorSendEmailType, ErrorTypeEnum> Map =
        new()
        {
            // Ошибки формата / валидации
            { ErrorSendEmailType.InvalidEmailFormat, ErrorTypeEnum.ValidationError },
            { ErrorSendEmailType.InvalidMessageContent, ErrorTypeEnum.ValidationError },
            { ErrorSendEmailType.AttachmentError, ErrorTypeEnum.ValidationError },

            // Ошибки аутентификации / прав доступа
            { ErrorSendEmailType.SmtpAuthenticationFailed, ErrorTypeEnum.ServerError },
            { ErrorSendEmailType.AccessDenied, ErrorTypeEnum.ServerError },
            { ErrorSendEmailType.InsufficientPermissions, ErrorTypeEnum.ServerError },

            // Ошибки сетевые / инфраструктурные
            { ErrorSendEmailType.SmtpServerUnavailable, ErrorTypeEnum.ServerError },
            { ErrorSendEmailType.NetworkError, ErrorTypeEnum.ServerError },
            { ErrorSendEmailType.SslHandshakeFailed, ErrorTypeEnum.ServerError },
            { ErrorSendEmailType.DnsResolutionFailed, ErrorTypeEnum.ServerError },
            { ErrorSendEmailType.OperationTimedOut, ErrorTypeEnum.ServerError },

            // Ошибки, связанные с почтовым ящиком
            { ErrorSendEmailType.MailboxDoesNotExist, ErrorTypeEnum.NotFound },
            { ErrorSendEmailType.MailboxFull, ErrorTypeEnum.Concurrency },

            // Ограничения провайдера
            { ErrorSendEmailType.SendingQuotaExceeded, ErrorTypeEnum.ServerError },
            { ErrorSendEmailType.MessageSizeLimitExceeded, ErrorTypeEnum.ServerError },
            { ErrorSendEmailType.BlockedAsSpam, ErrorTypeEnum.ServerError },

            // Внутренние сбои
            { ErrorSendEmailType.ServerError, ErrorTypeEnum.ServerError },
        };
    
    public static Result<T> ToResultFailure<T>(this SmtpSendRequest error) 
    {
        var message = error.Error?.GetDescription() ?? "Неизвестная ошибка";
        var type = Map.GetValueOrDefault(error.Error ?? ErrorSendEmailType.ServerError, ErrorTypeEnum.ServerError);

        return Result<T>.Failure(message, type);
    }
    
    private static string? GetDescription(this Enum value)
    {
        var field = value.GetType().GetField(value.ToString());
        var attr = field?.GetCustomAttribute<DescriptionAttribute>();
        return attr?.Description;
    }
}