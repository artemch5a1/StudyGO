using StudyGO.Core.Enums;
using StudyGO.Core.Extensions;

namespace StudyGO.Core.Models
{
    public class User
    {
        public User(
            Guid userId,
            string email,
            string passwordHash,
            RolesEnum role,
            string surname,
            string name,
            string patronymic,
            string? number
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
        }
        
        public User(
            string email,
            string passwordHash,
            string surname,
            string name,
            string patronymic,
            string? number
        )
        {
            Email = email.ToLower();
            PasswordHash = passwordHash;
            Surname = surname;
            Name = name;
            Patronymic = patronymic;
            Number = number;
        }
        
        public User(
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
        }
        
        public User(Guid userId, string email, string passwordHash)
        {
            UserId = userId;
            Email = email.ToLower();
            PasswordHash = passwordHash;
            Surname = String.Empty;
            Name = String.Empty;
            Patronymic = String.Empty;
            Number = String.Empty;
        }
        
        public User(
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
    }
}
