using System.ComponentModel.DataAnnotations.Schema;

namespace StudyGO.infrastructure.Entites
{
    public class UserEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid UserID { get; set; }

        public string Email { get; set; } = null!;

        public string PasswordHash { get; set; } = null!;

        public string Role { get; set; } = null!;

        public string Surname { get; set; } = null!;

        public string Name { get; set; } = null!;

        public string Patronymic { get; set; } = null!;

        public string? Number { get; set; }

        public virtual TutorProfileEntity? TutorProfile { get; set; }

        public virtual UserProfileEntity? UserProfile { get; set; }
    }
}
