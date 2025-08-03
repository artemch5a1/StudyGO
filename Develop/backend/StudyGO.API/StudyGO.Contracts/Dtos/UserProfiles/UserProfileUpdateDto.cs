using StudyGO.Contracts.Dtos.Subjects;

namespace StudyGO.Contracts.Dtos.UserProfiles
{
    public class UserProfileUpdateDto
    {
        public Guid UserID { get; set; }

        public DateTime DateBirth { get; set; }

        public Guid SubjectID { get; set; }

        public string Description { get; set; } = null!;
    }
}
