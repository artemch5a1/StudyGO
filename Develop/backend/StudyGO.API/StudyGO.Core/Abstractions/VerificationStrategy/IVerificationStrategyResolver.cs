using StudyGO.Contracts;

namespace StudyGO.Core.Abstractions.VerificationStrategy;

public interface IVerificationStrategyResolver
{
    IVerificationStrategy Resolve(RegistryScheme scheme);
}