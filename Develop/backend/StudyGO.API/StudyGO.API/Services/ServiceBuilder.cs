namespace StudyGO.API.Services
{
    public partial class ServiceBuilder
    {
        private readonly IConfiguration _configuration;

        private readonly IServiceCollection _services;

        public ServiceBuilder(IConfiguration configuration, IServiceCollection services )
        {
            _configuration = configuration;
            this._services = services;
        }

        public void BuildAllService()
        {
            ConfigureRepositories();
            ConfigureAutoMapper();
            ConfigureLogger();
            ConfigureJwtProvider();
            ConfigurePasswordHasher();
            ConfigureServicesApp();
            ConfigureValidators();
            ConfigureJwtAuthentication();
        }
    }
}
