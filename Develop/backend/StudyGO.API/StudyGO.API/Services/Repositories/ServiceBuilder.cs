using StudyGO.Core.Abstractions.Repositories;
using StudyGO.infrastructure.Repositories;

namespace StudyGO.API.Services
{
    public partial class ServiceBuilder
    {
        private void ConfigureRepositories()
        {
            _services.AddScoped<IUserRepository, UserRepository>();
            _services.AddScoped<IUserProfileRepository, UserProfileRepository>();
            _services.AddScoped<ITutorProfileRepository, TutorProfileRepository>();
            _services.AddScoped<IFormatRepository, FormatRepository>();
            _services.AddScoped<ISubjectRepository, SubjectRepository>();
        }
    }
}
