using StudyGO.Core.Abstractions.Utils;
using StudyGO.infrastructure.Utils;

namespace StudyGO.API.Services
{
    public partial class ServiceBuilder
    {
        private void ConfigurePasswordHasher()
        {
            _services.AddScoped<IPasswordHasher, PasswordHasher>();
        }
    }
}
