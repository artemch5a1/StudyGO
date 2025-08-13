namespace StudyGO.infrastructure.EmailServices;

public class EmailServiceOptions
{
    public string SmtpServer { get; set; } = null!;

    public int Port { get; set; }

    public string Fromname { get; set; } = null!;
    
    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;
}