namespace StudyGO.Core.Models
{
    public class UserProfile
    {
        public User User { get; set; } = null!;

        public DateTime DateBirth { get; set; }

        public Subject? FavoriteSubject { get; set; } = null;

        public string Description { get; set; } = null!;
    }
}
