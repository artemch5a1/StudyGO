using StudyGO.Core.Models;

namespace StudyGO.infrastructure.Entites
{
    public class TutorProfileEntity 
    {
        public Guid UserID { get; set; }
        public UserEntity User { get; set; } = null!;
        public string Bio { get; set; } = string.Empty;
        public decimal PricePerHour { get; set; } = decimal.Zero;
        public string City { get; set; } = string.Empty;
        public FormatEntity Format { get; set; } = null!;
    }
}
