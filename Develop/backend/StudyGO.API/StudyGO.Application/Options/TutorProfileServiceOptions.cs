using StudyGO.Contracts;

namespace StudyGO.Application.Options;

public class TutorProfileServiceOptions
{
    public bool RequireEmailVerification { get; set; }

    public RegistryScheme SchemeRegistry { get; set; } = RegistryScheme.VerifiedByLink;
}