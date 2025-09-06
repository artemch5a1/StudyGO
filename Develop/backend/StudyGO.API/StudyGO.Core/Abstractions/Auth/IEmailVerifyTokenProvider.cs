namespace StudyGO.Core.Abstractions.Auth;

public interface IEmailVerifyTokenProvider
{
    string GenerateToken(Guid userId);
}