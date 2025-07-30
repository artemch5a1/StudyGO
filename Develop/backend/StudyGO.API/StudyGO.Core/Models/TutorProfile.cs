namespace StudyGO.Core.Models
{
    public class TutorProfile 
    {
        public User User { get; set; } = null!;
        public string Bio { get; set; } = string.Empty;
        public decimal PricePerHour { get; set; } = decimal.Zero;
        public string City { get; set; } = string.Empty;
        public Format Format { get; set; } = null!;
    }
}
