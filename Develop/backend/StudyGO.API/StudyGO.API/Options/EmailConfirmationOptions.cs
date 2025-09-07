namespace StudyGO.API.Options;

public class EmailConfirmationOptions
{
    public string Action { get; set; } = null!;
    public string Controller { get; set; } = null!;

    public string ConfirmAction { get; set; } = null!;
}