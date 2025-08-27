using StudyGO.Contracts.Dtos.Users;
using StudyGO.Contracts.ValidatableMarker;

namespace StudyGO.Contracts.Dtos.UserProfiles
{
    public class UserProfileRegistrDto : IValidatable
    {
        public UserCreateDto User { get; set; } = null!;

        public DateOnly DateBirth { get; set; }

        public Guid? SubjectId { get; set; } = null;

        public string Description { get; set; } = null!;
    }
}
