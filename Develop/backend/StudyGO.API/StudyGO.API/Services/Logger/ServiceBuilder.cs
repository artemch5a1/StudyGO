namespace StudyGO.API.Services;

public partial class ServiceBuilder
{
    private static void ConfigureLogger(IServiceCollection services)
    {
        services.AddLogging(config =>
        {
            config.AddDebug();
            config.SetMinimumLevel(LogLevel.Information);
        });
    }
}
