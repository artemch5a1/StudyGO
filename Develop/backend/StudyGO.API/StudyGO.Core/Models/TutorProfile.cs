namespace StudyGO.Core.Models
{
    public class TutorProfile 
    {
        public Guid UserId { get; set; }

        public User? User { get; set; }

        public string Bio { get; set; } = string.Empty;

        public decimal PricePerHour { get; set; } = decimal.Zero;

        public string City { get; set; } = string.Empty;

        public int FormatId { get; set; }

        public Format? Format { get; set; }

        public ICollection<TutorSubjects> TutorSubjects { get; set; } = [];
    }
}
