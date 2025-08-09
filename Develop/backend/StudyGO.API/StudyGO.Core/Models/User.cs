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
            Email = email;
            PasswordHash = passwordHash;
            Role = role.GetString();
            Surname = surname;
            Name = name;
            Patronymic = patronymic;
            Number = number;
        }

        public User(Guid userId)
        {
            UserId = userId;
            Email = string.Empty;
            PasswordHash = string.Empty;
            Role = string.Empty;
            Surname = string.Empty;
            Name = string.Empty;
            Patronymic = string.Empty;
        }

        public User() { }

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
