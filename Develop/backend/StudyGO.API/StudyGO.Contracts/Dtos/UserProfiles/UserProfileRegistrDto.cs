using StudyGO.Contracts.Dtos.Subjects;
using StudyGO.Contracts.Dtos.Users;

namespace StudyGO.Contracts.Dtos.UserProfiles
{
    public class UserProfileRegistrDto 
    {
        public UserCreateDto userCreateDto { get; set; } = null!;

        public DateTime DateBirth { get; set; }

        public SubjectDto? FavoriteSubject { get; set; } = null;

        public string Description { get; set; } = null!;
    }
}
