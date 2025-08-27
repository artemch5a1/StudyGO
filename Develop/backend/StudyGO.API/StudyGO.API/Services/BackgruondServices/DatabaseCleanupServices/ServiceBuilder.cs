using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection.Extensions;
using StudyGO.Application.Services.Account;
using StudyGO.Core.Abstractions.CleanupServices;

namespace StudyGO.API.Services;

public partial class ServiceBuilder
{
    private void ConfigureDataBaseCleanupServices()
    {
        AddCleanupService<UserVerificationCleanup>();
    }
    
    private void AddCleanupService<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TCleanupService>()
        where TCleanupService : class, IDatabaseCleanupService
    {
        _services.TryAddEnumerable(ServiceDescriptor.Scoped<IDatabaseCleanupService, TCleanupService>());
    }
}