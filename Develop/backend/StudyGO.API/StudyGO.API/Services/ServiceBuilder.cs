namespace StudyGO.API.Services
{
    public static partial class ServiceBuilder
    {
        public static void BuildAllService(IServiceCollection services)
        {
            ConfigureRepositories(services);
            ConfigureAutoMapper(services);
            ConfigureLogger(services);
        }
    }
}
