using StudyGO.Contracts.Dtos.Users;
using StudyGO.Contracts.ValidatableMarker;

namespace StudyGO.Contracts.Dtos.TutorProfiles
{
    public class TutorProfileRegistrDto : IValidatable
    {
        public UserCreateDto User { get; set; } = null!;

        public string Bio { get; set; } = string.Empty;

        public decimal PricePerHour { get; set; } = decimal.Zero;

        public string City { get; set; } = string.Empty;

        public int FormatId { get; set; }

        public List<Guid> SubjectsId { get; set; } = [];
    }
}
