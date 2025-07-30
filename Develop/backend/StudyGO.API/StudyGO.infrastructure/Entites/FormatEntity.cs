using System.ComponentModel.DataAnnotations.Schema;

namespace StudyGO.infrastructure.Entites
{
    public class FormatEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int FormatID { get; set; }
        public string Title { get; set; } = null!;
        public virtual ICollection<TutorProfileEntity> TutorProfiles { get; set; } =
            new List<TutorProfileEntity>();
    }
}
