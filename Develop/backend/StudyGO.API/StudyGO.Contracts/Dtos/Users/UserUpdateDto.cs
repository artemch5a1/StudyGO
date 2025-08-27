using StudyGO.Contracts.ValidatableMarker;

namespace StudyGO.Contracts.Dtos.Users
{
    public class UserUpdateDto : IValidatable
    {
        public Guid UserId { get; set; }

        public string Surname { get; set; } = null!;

        public string Name { get; set; } = null!;

        public string Patronymic { get; set; } = null!;

        public string Number { get; set; } = string.Empty;
    }
}
