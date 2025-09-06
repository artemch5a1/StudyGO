namespace StudyGO.Contracts.Contracts;

public class UserRegistryResponse
{
    protected UserRegistryResponse(
        Guid userId,
        bool requireEmailVerification,
        RegistryScheme scheme = RegistryScheme.DefaultVerified
        )
    {
        UserId = userId;
        SchemeRegistry = scheme;
        RequireEmailVerification = requireEmailVerification;
    }
    
    public Guid UserId { get; set; }

    public bool RequireEmailVerification { get; set; } = false;

    public RegistryScheme SchemeRegistry { get; set; }

    public static UserRegistryResponse WithoutVerified(Guid userId)
    {
        return new UserRegistryResponse(userId, false, RegistryScheme.DefaultVerified);
    }

    public static UserRegistryResponse VerifiedByLink(Guid userId)
    {
        return new UserRegistryResponse(userId, true, RegistryScheme.VerifiedByLink);
    }

    public static UserRegistryResponse VerifiedByAnotherScheme(Guid userId, RegistryScheme scheme)
    {
        return new UserRegistryResponse(userId, true, scheme);
    }
}