using System.ComponentModel;

namespace StudyGO.Core.Enums;

public enum ErrorSendEmailType
{
    [Description("Неверный формат email")]
    InvalidEmailFormat,
    
    [Description("Сервер почты недоступен")]
    SmtpServerUnavailable,
    
    [Description("Ошибка авторизации на почтовом сервере")]
    SmtpAuthenticationFailed,
    
    [Description("Почтовый ящик не существует")]
    MailboxDoesNotExist,
    
    [Description("Превышена квота отправки")]
    SendingQuotaExceeded,
    
    [Description("Превышен максимальный размер сообщения")]
    MessageSizeLimitExceeded,
    
    [Description("Таймаут операции")]
    OperationTimedOut,
    
    [Description("Ошибка сети")]
    NetworkError,
    
    [Description("Ошибка безопасного соединения")]
    SslHandshakeFailed,
    
    [Description("Отказ в доступе")]
    AccessDenied,
    
    [Description("Внутренняя ошибка сервера")]
    ServerError,
    
    [Description("Ошибка обработки вложения")]
    AttachmentError,
    
    [Description("Ошибка DNS")]
    DnsResolutionFailed,
    
    [Description("Почтовый ящик переполнен")]
    MailboxFull,
    
    [Description("Сообщение заблокировано как спам")]
    BlockedAsSpam,
    
    [Description("Недостаточно прав для отправки")]
    InsufficientPermissions,
    
    [Description("Недопустимые символы в теле письма")]
    InvalidMessageContent
}