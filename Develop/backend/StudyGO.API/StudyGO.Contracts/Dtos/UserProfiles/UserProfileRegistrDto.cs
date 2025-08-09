using StudyGO.Contracts.Dtos.Subjects;
using StudyGO.Contracts.Dtos.Users;

namespace StudyGO.Contracts.Dtos.UserProfiles
{
    public class UserProfileRegistrDto
    {
        public UserCreateDto User { get; set; } = null!;

        public DateOnly DateBirth { get; set; }

        public Guid? SubjectId { get; set; } = null;

        public string Description { get; set; } = null!;
    }
}
