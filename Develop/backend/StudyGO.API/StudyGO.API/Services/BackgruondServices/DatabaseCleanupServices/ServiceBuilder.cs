using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection.Extensions;
using StudyGO.API.Options;
using StudyGO.Core.Abstractions.CleanupServices;
using StudyGO.infrastructure.Data.CleanUpServices;
using StudyGO.infrastructure.Data.CleanUpServices.Options;

namespace StudyGO.API.Services;

public partial class ServiceBuilder
{
    private void ConfigureDataBaseCleanupServices()
    {
        _services.Configure<UserVerificationCleanUpOptions>(
            _configuration.GetSection("UserVerificationCleanUpOptions"));

        _services.Configure<DatabaseCleanerOptions>(_configuration.GetSection("DatabaseCleanerOptions"));
        
        AddCleanupService<UserVerificationCleanup>();
    }
    
    private void AddCleanupService<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TCleanupService>()
        where TCleanupService : class, IDatabaseCleanupService
    {
        _services.TryAddEnumerable(ServiceDescriptor.Scoped<IDatabaseCleanupService, TCleanupService>());
    }
}