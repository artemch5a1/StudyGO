using StudyGO.Contracts.ValidatableMarker;

namespace StudyGO.Contracts.Dtos.TutorProfiles
{
    public class TutorProfileUpdateDto : IValidatable
    {
        public Guid UserId { get; set; }

        public string Bio { get; set; } = string.Empty;

        public decimal PricePerHour { get; set; } = decimal.Zero;

        public string City { get; set; } = string.Empty;

        public int FormatId { get; set; }
        
        public List<Guid> SubjectsId { get; set; } = [];
    }
}
