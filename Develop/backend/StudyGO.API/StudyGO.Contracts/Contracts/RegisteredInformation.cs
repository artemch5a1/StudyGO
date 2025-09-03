namespace StudyGO.Contracts.Contracts;

public record RegisteredInformation(Guid UserId, 
    string ConfirmEndpoint, 
    string Role, 
    RegistryScheme Scheme);