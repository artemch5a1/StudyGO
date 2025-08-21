using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection.Extensions;
using StudyGO.Core.Abstractions.DatabaseSeed;
using StudyGO.infrastructure.Data;
using StudyGO.infrastructure.Data.SeedServices;
using StudyGO.infrastructure.Data.SeedServices.SeedOptions;

namespace StudyGO.API.Services;

public partial class ServiceBuilder
{
    private void ConfigureSeederAndServices()
    {
        _services.AddScoped<ISeedProvider, Seeder>();
        
        _services.Configure<SubjectSeedOptions>(_configuration.GetSection("SeedData"));
        
        AddSeedService<SubjectsSeeder>();

        _services.Configure<FormatSeedOptions>(_configuration.GetSection("SeedData"));
        
        AddSeedService<FormatsSeeder>();
    }
    
    private void AddSeedService<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TSeedService>()
        where TSeedService : class, ISeedService
    {
        _services.TryAddEnumerable(ServiceDescriptor.Scoped<ISeedService, TSeedService>());
    }
}