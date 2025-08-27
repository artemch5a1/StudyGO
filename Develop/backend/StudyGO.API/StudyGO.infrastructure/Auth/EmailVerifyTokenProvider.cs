using System.Security.Cryptography;
using System.Text;
using StudyGO.Core.Abstractions.Auth;

namespace StudyGO.infrastructure.Auth;

public class EmailVerifyTokenProvider : IEmailVerifyTokenProvider
{
    private const string TokenSecret = "your-secret-key";
    private const int TokenLength = 32;
    
    public string GenerateToken(Guid userId)
    {
        using var sha256 = SHA256.Create();
        byte[] inputBytes = Encoding.UTF8.GetBytes(userId.ToString() + TokenSecret);
        byte[] hashBytes = sha256.ComputeHash(inputBytes);
        
        string token = Convert.ToBase64String(hashBytes)
            .Replace("+", "-")
            .Replace("/", "_")
            .Replace("=", "")
            .Substring(0, TokenLength);
        
        return token;
    }
}