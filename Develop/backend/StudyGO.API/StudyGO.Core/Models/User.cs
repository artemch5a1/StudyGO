using StudyGO.Core.Enums;
using StudyGO.Core.Extensions;

namespace StudyGO.Core.Models
{
    public class User
    {
        private User(
            Guid userId,
            string email,
            string passwordHash,
            RolesEnum role,
            string surname,
            string name,
            string patronymic,
            string? number,
            DateTime dateRegistry,
            bool verified,
            DateTime? verifiedDate
        )
        {
            UserId = userId;
            Email = email.ToLower();
            PasswordHash = passwordHash;
            Role = role.GetString();
            Surname = surname;
            Name = name;
            Patronymic = patronymic;
            Number = number;
            DateRegistry = dateRegistry;
            Verified = verified;
            VerifiedDate = verifiedDate;
        }
        
        private User(
            string email,
            string passwordHash,
            string surname,
            string name,
            string patronymic,
            string? number,
            RolesEnum role
        )
        {
            Email = email.ToLower();
            PasswordHash = passwordHash;
            Surname = surname;
            Name = name;
            Patronymic = patronymic;
            Number = number;
            Role = role.GetString();
            Verified = false;
            DateRegistry = DateTime.UtcNow;
            VerifiedDate = null;
        }
        
        private User(Guid userId, string email, string passwordHash)
        {
            UserId = userId;
            Email = email.ToLower();
            PasswordHash = passwordHash;
            Surname = String.Empty;
            Name = String.Empty;
            Patronymic = String.Empty;
            Number = String.Empty;
        }
        
        private User(
            Guid userId,
            string surname,
            string name,
            string patronymic,
            string? number
        )
        {
            UserId = userId;
            Surname = surname;
            Name = name;
            Patronymic = patronymic;
            Number = number;
        }

        public Guid UserId { get; set; }

        public string Email { get; set; } = null!;

        public string PasswordHash { get; set; } = null!;

        public string Role { get; set; } = null!;

        public string Surname { get; set; } = null!;

        public string Name { get; set; } = null!;

        public string Patronymic { get; set; } = null!;

        public string? Number { get; set; }
        
        public DateTime DateRegistry { get; set; }

        public bool Verified { get; set; }

        public string? VerifiedToken { get; set; } = null;

        public DateTime? VerifiedDate { get; set; }

        public static User MapToUserFromEntity
        (
            Guid userId,
            string email,
            string passwordHash,
            RolesEnum role,
            string surname,
            string name,
            string patronymic,
            string? number,
            DateTime dateRegistry,
            bool verified,
            DateTime? verifiedDate
            )
        {
            return new User(
                userId,
                email,
                passwordHash,
                role,
                surname,
                name,
                patronymic,
                number,
                dateRegistry,
                verified,
                verifiedDate
            );
        }

        public static User CreateUser(
            string email,
            string passwordHash,
            string surname,
            string name,
            string patronymic,
            string? number,
            RolesEnum role
        )
        {
            return new User(email, passwordHash, surname, name, patronymic, number, role);
        }

        public static User UpdateUser(
            Guid userId,
            string surname,
            string name,
            string patronymic,
            string? number
            )
        {
            return new User(userId, surname, name, patronymic, number);
        }

        public static User UpdateUserCredentials(Guid userId, string email, string passwordHash)
        {
            return new User(userId, email, passwordHash);
        }
    }
}
