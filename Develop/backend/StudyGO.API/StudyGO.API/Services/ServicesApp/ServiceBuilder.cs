using StudyGO.Application.Extensions;
using StudyGO.Application.Options;
using StudyGO.Application.Services.Account;
using StudyGO.Core.Abstractions.Services.Account;
using StudyGO.Core.Abstractions.VerificationStrategy;
using StudyGO.infrastructure.VerificationStrategy;
using StudyGO.infrastructure.VerificationStrategy.Resolver;

namespace StudyGO.API.Services
{
    public partial class ServiceBuilder
    {
        private void ConfigureServicesApp()
        {
            _services.Configure<UserProfileServiceOptions>(_configuration.GetSection("UserAccount"));
            _services.Configure<TutorProfileServiceOptions>(_configuration.GetSection("TutorAccount"));
            
            _services.AddScoped<IUserProfileService, UserProfileService>();

            ConfigureVerification();
            
            ConfigureMediatr();
        }

        private void ConfigureMediatr()
        {
            _services.AddApplication();
        }

        private void ConfigureVerification()
        {
            _services.AddScoped<IVerificationStrategyResolver, VerificationStrategyResolver>();

            _services.AddScoped<IVerificationStrategy, EmailVerificationStrategy>();

            _services.AddScoped<IVerificationStrategy, DefaultVerificationStrategy>();
        }
    }
}
