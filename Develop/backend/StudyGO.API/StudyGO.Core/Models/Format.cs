namespace StudyGO.Core.Models
{
    public class Format
    {
        public int FormatId { get; set; }

        public string Title { get; set; } = null!;

        public ICollection<TutorProfile> UserProfiles { get; set; } = [];
    }
}
