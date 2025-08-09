using StudyGO.Contracts.Dtos.Subjects;

namespace StudyGO.Contracts.Dtos.UserProfiles
{
    public class UserProfileUpdateDto
    {
        public Guid UserId { get; set; }

        public DateOnly DateBirth { get; set; }

        public Guid SubjectId { get; set; }

        public string Description { get; set; } = null!;
    }
}
