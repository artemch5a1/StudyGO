using StudyGO.Application.Helpers;
using StudyGO.Core.Abstractions.Utils;

namespace StudyGO.API.Services
{
    public partial class ServiceBuilder
    {
        public void ConfigurePasswordHasher()
        {
            _services.AddScoped<IPasswordHasher, PasswordHasher>();
        }
    }
}
