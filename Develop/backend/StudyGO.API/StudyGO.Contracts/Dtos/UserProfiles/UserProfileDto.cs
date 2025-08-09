using StudyGO.Contracts.Dtos.Subjects;
using StudyGO.Contracts.Dtos.Users;

namespace StudyGO.Contracts.Dtos.UserProfiles
{
    public class UserProfileDto
    {
        public UserDto User { get; set; } = null!;

        public DateOnly DateBirth { get; set; }

        public SubjectDto? FavoriteSubject { get; set; } = null;

        public string Description { get; set; } = null!;
    }
}
