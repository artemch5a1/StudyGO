using MediatR;
using Microsoft.Extensions.DependencyInjection;
using StudyGO.Application.UseCases.PipelineBehaviors;

namespace StudyGO.Application.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => 
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));
        
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ResultEnforcementBehavior<,>));
        
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        
        return services;
    }
}