namespace StudyGO.Contracts.Contracts;

public record ConfirmEmailRequest(Guid UserId, string Token);