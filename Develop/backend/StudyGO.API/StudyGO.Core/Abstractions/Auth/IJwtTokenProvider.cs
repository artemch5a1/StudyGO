using StudyGO.Contracts.Contracts;

namespace StudyGO.Core.Abstractions.Auth
{
    public interface IJwtTokenProvider 
    {
        string GenerateToken(UserLoginResponse user);
    }
}
