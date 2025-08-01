using StudyGO.Application.Abstractions;

namespace StudyGO.Application.Helpers
{
    public class PasswordHasher : IPasswordHasher
    {
        public string GeneratePasswordHash(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifiyPassword(string password, string passwordHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }
    }
}
