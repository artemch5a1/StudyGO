using StudyGO.Core.Abstractions.Repositories;
using StudyGO.infrastructure.Repositories;

namespace StudyGO.API.Services
{
    public partial class ServiceBuilder
    {
        private void ConfigureRepositories()
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserProfileRepository, UserProfileRepository>();
            services.AddScoped<ITutorProfileRepository, TutorProfileRepository>();
            services.AddScoped<IFormatRepository, FormatRepository>();
            services.AddScoped<ISubjectRepository, SubjectRepository>();
        }
    }
}
