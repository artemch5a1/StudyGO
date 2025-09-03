using StudyGO.Contracts;
using StudyGO.Core.Abstractions.VerificationStrategy;

namespace StudyGO.infrastructure.VerificationStrategy.Resolver;

public class VerificationStrategyResolver : IVerificationStrategyResolver
{
    private readonly IEnumerable<IVerificationStrategy> _strategies;

    public VerificationStrategyResolver(IEnumerable<IVerificationStrategy> strategies)
    {
        _strategies = strategies;
    }

    public IVerificationStrategy Resolve(RegistryScheme scheme)
    {
        var strategy = _strategies.FirstOrDefault(s => s.Scheme == scheme);
        if (strategy is null)
            throw new InvalidOperationException($"Нет стратегии для схемы {scheme}");

        return strategy;
    }
}