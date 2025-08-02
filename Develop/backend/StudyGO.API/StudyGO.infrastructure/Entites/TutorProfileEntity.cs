using StudyGO.Core.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudyGO.infrastructure.Entites
{
    public class TutorProfileEntity
    {
        public Guid UserID { get; set; }

        [ForeignKey(nameof(UserID))]
        public virtual UserEntity? User { get; set; }

        public string Bio { get; set; } = string.Empty;

        public decimal PricePerHour { get; set; } = decimal.Zero;

        public string City { get; set; } = string.Empty;

        public int FormatID { get; set; }

        [ForeignKey(nameof(FormatID))]
        public virtual FormatEntity? Format { get; set; }
    }
}
