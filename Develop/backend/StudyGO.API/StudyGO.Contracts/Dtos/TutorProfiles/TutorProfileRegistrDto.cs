using StudyGO.Contracts.Dtos.Formats;
using StudyGO.Contracts.Dtos.Users;

namespace StudyGO.Contracts.Dtos.TutorProfiles
{
    public class TutorProfileRegistrDto 
    {
        public UserCreateDto User { get; set; } = null!;

        public string Bio { get; set; } = string.Empty;

        public decimal PricePerHour { get; set; } = decimal.Zero;

        public string City { get; set; } = string.Empty;

        public int FormatID { get; set; }
    }
}
