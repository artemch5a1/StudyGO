using System.ComponentModel.DataAnnotations.Schema;

namespace StudyGO.infrastructure.Entities
{
    public class UserEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid UserId { get; set; }

        public string Email { get; set; } = null!;

        public string PasswordHash { get; set; } = null!;

        public string Role { get; set; } = null!;

        public string Surname { get; set; } = null!;

        public string Name { get; set; } = null!;

        public string Patronymic { get; set; } = null!;

        public string? Number { get; set; }

        public virtual TutorProfileEntity? TutorProfile { get; set; }

        public virtual UserProfileEntity? UserProfile { get; set; }

        public DateTime DateRegistry { get; set; }

        public bool Verified { get; set; }

        public string? VerifiedToken { get; set; } = null;
        
        public DateTime? VerifiedDate { get; set; } = null;
    }
}
