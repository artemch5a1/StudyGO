using StudyGO.Application.Options;
using StudyGO.Application.Services;
using StudyGO.Application.Services.Account;
using StudyGO.Core.Abstractions.Services;
using StudyGO.Core.Abstractions.Services.Account;

namespace StudyGO.API.Services
{
    public partial class ServiceBuilder
    {
        private void ConfigureServicesApp()
        {
            _services.Configure<UserProfileServiceOptions>(_configuration.GetSection("UserAccount"));
            
            _services.AddScoped<IUserAccountService, UserAccountService>();
            _services.AddScoped<IUserProfileService, UserProfileService>();
            _services.AddScoped<ITutorProfileService, TutorProfileService>();
            _services.AddScoped<IFormatService, FormatService>();
            _services.AddScoped<ISubjectService, SubjectService>();
        }
    }
}
