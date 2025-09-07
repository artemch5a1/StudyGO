using StudyGO.Contracts.ValidatableMarker;

namespace StudyGO.Contracts.Dtos.Users
{
    public class UserUpdatePasswordDto : IValidatable
    {
        public Guid UserId { get; set; }

        public string Password { get; set; } = null!;

        public string OldPassword { get; set; } = null!;
    }
}
