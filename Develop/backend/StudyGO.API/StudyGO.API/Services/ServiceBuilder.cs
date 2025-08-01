namespace StudyGO.API.Services
{
    public partial class ServiceBuilder
    {
        private readonly IConfiguration _configuration;

        private readonly IServiceCollection services;

        public ServiceBuilder(IConfiguration configuration, IServiceCollection services )
        {
            _configuration = configuration;
            this.services = services;
        }

        public void BuildAllService()
        {
            ConfigureRepositories();
            ConfigureAutoMapper();
            ConfigureLogger();
            ConfigureJwtProvider();
            ConfigurePasswordHasher();
        }
    }
}
