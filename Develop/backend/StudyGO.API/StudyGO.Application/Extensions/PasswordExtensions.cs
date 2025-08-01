using StudyGO.Application.Abstractions;

namespace StudyGO.Application.Extensions
{
    public static class PasswordExtensions
    {
        public static string HashedPassword(this string password, IPasswordHasher passwordHasher)
        {
            return passwordHasher.GeneratePasswordHash(password);
        }
    }
}
