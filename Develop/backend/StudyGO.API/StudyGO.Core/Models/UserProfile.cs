namespace StudyGO.Core.Models
{
    public class UserProfile
    {
        public Guid UserID { get; set; }

        public User? User { get; set; }

        public DateTime DateBirth { get; set; }

        public Guid? SubjectID { get; set; }

        public Subject? FavoriteSubject { get; set; } = null;

        public string Description { get; set; } = null!;
    }
}
