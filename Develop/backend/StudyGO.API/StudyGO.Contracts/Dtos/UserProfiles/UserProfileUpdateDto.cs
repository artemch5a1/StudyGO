using StudyGO.Contracts.ValidatableMarker;

namespace StudyGO.Contracts.Dtos.UserProfiles
{
    public class UserProfileUpdateDto : IValidatable
    {
        public Guid UserId { get; set; }

        public DateOnly DateBirth { get; set; }

        public Guid SubjectId { get; set; }

        public string Description { get; set; } = null!;
    }
}
