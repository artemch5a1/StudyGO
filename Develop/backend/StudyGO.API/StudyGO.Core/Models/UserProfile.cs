namespace StudyGO.Core.Models
{
    public class UserProfile
    {
        public Guid UserId { get; set; }

        public User? User { get; set; }

        public DateOnly DateBirth { get; set; }

        public Guid? SubjectId { get; set; }

        public Subject? FavoriteSubject { get; set; } = null;

        public string Description { get; set; } = null!;
    }
}
