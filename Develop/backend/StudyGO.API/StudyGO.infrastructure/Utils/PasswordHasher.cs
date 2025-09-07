using StudyGO.Core.Abstractions.Utils;

namespace StudyGO.infrastructure.Utils
{
    public class PasswordHasher : IPasswordHasher
    {
        public string GeneratePasswordHash(string password) =>
            BCrypt.Net.BCrypt.EnhancedHashPassword(password);

        public bool VerifiyPassword(string password, string passwordHash) =>
            BCrypt.Net.BCrypt.EnhancedVerify(password, passwordHash);
    }
}
