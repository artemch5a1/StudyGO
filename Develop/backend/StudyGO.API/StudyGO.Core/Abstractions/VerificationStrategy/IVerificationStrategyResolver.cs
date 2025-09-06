using StudyGO.Contracts;
using StudyGO.Core.Abstractions.VerificationStrategy;

namespace StudyGO.Core.Abstractions.VerificationStrategy;

public interface IVerificationStrategyResolver
{
    IVerificationStrategy Resolve(RegistryScheme scheme);
}