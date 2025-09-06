using System.ComponentModel.DataAnnotations.Schema;

namespace StudyGO.infrastructure.Entities
{
    public class UserProfileEntity
    {
        public Guid UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual UserEntity? User { get; set; } = null!;

        public DateOnly DateBirth { get; set; }

        public Guid? SubjectId { get; set; } = null;


        [ForeignKey(nameof(SubjectId))]
        public virtual SubjectEntity? FavoriteSubject { get; set; } = null;

        public string Description { get; set; } = null!;
    }
}
