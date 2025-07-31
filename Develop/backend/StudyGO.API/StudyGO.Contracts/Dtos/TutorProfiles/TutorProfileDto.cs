using StudyGO.Contracts.Dtos.Formats;
using StudyGO.Contracts.Dtos.Users;

namespace StudyGO.Contracts.Dtos.TutorProfiles
{
    public class TutorProfileDto 
    {
        public UserDto User { get; set; } = null!;

        public string Bio { get; set; } = string.Empty;

        public decimal PricePerHour { get; set; } = decimal.Zero;

        public string City { get; set; } = string.Empty;

        public FormatDto Format { get; set; } = null!;
    }
}
