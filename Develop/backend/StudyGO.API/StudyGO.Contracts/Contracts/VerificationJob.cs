namespace StudyGO.Contracts.Contracts;

public record VerificationJob(
    Guid UserId,
    string Email,
    string ConfirmEmailEndpoint
);