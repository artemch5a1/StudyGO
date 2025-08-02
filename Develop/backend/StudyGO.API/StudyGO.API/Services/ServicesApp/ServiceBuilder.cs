using StudyGO.Application.Services.Account;
using StudyGO.Core.Abstractions.Services.Account;

namespace StudyGO.API.Services
{
    public partial class ServiceBuilder
    {
        public void ConfigureServicesApp()
        {
            services.AddScoped<IUserAccountService, UserAccountService>();
            services.AddScoped<IUserProfileService, UserProfileService>();
        }
    }
}
