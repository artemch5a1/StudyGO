using StudyGO.Contracts.Dtos.Subjects;

namespace StudyGO.Contracts.Dtos.UserProfiles
{
    public class UserProfileUpdateDto
    {
        public DateTime DateBirth { get; set; }

        public SubjectDto? FavoriteSubject { get; set; } = null;

        public string Description { get; set; } = null!;
    }
}
