using Microsoft.Extensions.DependencyInjection;
using StudyGO.Contracts;
using StudyGO.Core.Abstractions.VerificationStrategy;

namespace StudyGO.infrastructure.VerificationStrategy.Resolver;

public class VerificationStrategyResolver : IVerificationStrategyResolver
{
    private readonly IServiceProvider _provider;

    public VerificationStrategyResolver(IServiceProvider provider)
    {
        _provider = provider;
    }

    public IVerificationStrategy Resolve(RegistryScheme scheme)
    {
        var strategies = _provider.GetServices<IVerificationStrategy>();
        
        var strategy = strategies.FirstOrDefault(s => s.Scheme == scheme);
        if (strategy is null)
            throw new InvalidOperationException($"Нет стратегии для схемы {scheme}");

        return strategy;
    }
}