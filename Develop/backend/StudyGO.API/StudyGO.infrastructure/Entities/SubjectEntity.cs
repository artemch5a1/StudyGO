using System.ComponentModel.DataAnnotations.Schema;

namespace StudyGO.infrastructure.Entities
{
    public class SubjectEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid SubjectId { get; set; }

        public string Title { get; set; } = null!;

        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Пользователи, которые посчитали этот предмет любимым
        /// </summary>
        public virtual ICollection<UserProfileEntity> UserProfiles { get; set; } = [];

        public virtual ICollection<TutorSubjectsEntity> TutorSubjects { get; set; } = [];
    }
}
