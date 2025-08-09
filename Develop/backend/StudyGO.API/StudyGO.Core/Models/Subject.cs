namespace StudyGO.Core.Models
{
    public class Subject
    {
        public Guid SubjectId { get; set; }

        public string Title { get; set; } = null!;

        public string Description { get; set; } = string.Empty;

        public ICollection<UserProfile> UserProfiles { get; set; } = [];
    }
}
