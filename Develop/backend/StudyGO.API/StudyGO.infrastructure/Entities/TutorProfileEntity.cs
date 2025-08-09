using System.ComponentModel.DataAnnotations.Schema;

namespace StudyGO.infrastructure.Entities
{
    public class TutorProfileEntity
    {
        public Guid UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual UserEntity? User { get; set; }

        public string Bio { get; set; } = string.Empty;

        public decimal PricePerHour { get; set; } = decimal.Zero;

        public string City { get; set; } = string.Empty;

        public int FormatId { get; set; }

        [ForeignKey(nameof(FormatId))]
        public virtual FormatEntity? Format { get; set; }

        public virtual ICollection<TutorSubjectsEntity> TutorSubjects { get; set; } = [];
    }
}
