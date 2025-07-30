using System.ComponentModel.DataAnnotations.Schema;

namespace StudyGO.infrastructure.Entites
{
    public class SubjectEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid SubjectID { get; set; }

        public string Title { get; set; } = null!;

        public string Description { get; set; } = string.Empty;

        public virtual ICollection<UserProfileEntity> UserProfiles { get; set; } =
            new List<UserProfileEntity>();
    }
}
