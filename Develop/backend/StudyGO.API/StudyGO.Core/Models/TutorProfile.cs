namespace StudyGO.Core.Models
{
    public class TutorProfile 
    {
        public Guid UserID { get; set; }

        public User? User { get; set; }

        public string Bio { get; set; } = string.Empty;

        public decimal PricePerHour { get; set; } = decimal.Zero;

        public string City { get; set; } = string.Empty;

        public int FormatID { get; set; }

        public Format? Format { get; set; }
    }
}
