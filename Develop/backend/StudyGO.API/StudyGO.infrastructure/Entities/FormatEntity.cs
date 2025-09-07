using System.ComponentModel.DataAnnotations.Schema;

namespace StudyGO.infrastructure.Entities
{
    public class FormatEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int FormatId { get; set; }
        public string Title { get; set; } = null!;
        public virtual ICollection<TutorProfileEntity> TutorProfiles { get; set; } =
            new List<TutorProfileEntity>();
    }
}
