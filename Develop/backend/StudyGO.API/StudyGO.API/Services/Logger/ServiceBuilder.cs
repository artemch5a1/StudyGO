namespace StudyGO.API.Services;

public partial class ServiceBuilder
{
    private void ConfigureLogger()
    {
        services.AddLogging(config =>
        {
            config.AddDebug();
            config.SetMinimumLevel(LogLevel.Information);
        });
    }
}
