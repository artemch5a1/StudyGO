using System.ComponentModel;

namespace StudyGO.Core.Enums;

public enum ErrorSendEmailType
{
    [Description("�������� ������ email")]
    InvalidEmailFormat,
    
    [Description("������ ����� ����������")]
    SmtpServerUnavailable,
    
    [Description("������ ����������� �� �������� �������")]
    SmtpAuthenticationFailed,
    
    [Description("�������� ���� �� ����������")]
    MailboxDoesNotExist,
    
    [Description("��������� ����� ��������")]
    SendingQuotaExceeded,
    
    [Description("�������� ������������ ������ ���������")]
    MessageSizeLimitExceeded,
    
    [Description("������� ��������")]
    OperationTimedOut,
    
    [Description("������ ����")]
    NetworkError,
    
    [Description("������ ����������� ����������")]
    SslHandshakeFailed,
    
    [Description("����� � �������")]
    AccessDenied,
    
    [Description("���������� ������ �������")]
    ServerError,
    
    [Description("������ ��������� ��������")]
    AttachmentError,
    
    [Description("������ DNS")]
    DnsResolutionFailed,
    
    [Description("�������� ���� ����������")]
    MailboxFull,
    
    [Description("��������� ������������� ��� ����")]
    BlockedAsSpam,
    
    [Description("������������ ���� ��� ��������")]
    InsufficientPermissions,
    
    [Description("������������ ������� � ���� ������")]
    InvalidMessageContent
}