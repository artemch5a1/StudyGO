using StudyGO.Contracts;

namespace StudyGO.Application.Options;

public class TutorProfileServiceOptions
{
    public bool RequireEmailVerification { get; set; }

    public RegistryScheme SchemeReqistry { get; set; } = RegistryScheme.VerifiedByLink;
}