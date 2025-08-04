using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

namespace StudyGO.infrastructure.Entites
{
    public class UserProfileEntity
    {
        public Guid UserID { get; set; }

        [ForeignKey(nameof(UserID))]
        public virtual UserEntity? User { get; set; } = null!;

        public DateOnly DateBirth { get; set; }

        public Guid? SubjectID { get; set; } = null;


        [ForeignKey(nameof(SubjectID))]
        public virtual SubjectEntity? FavoriteSubject { get; set; } = null;

        public string Description { get; set; } = null!;
    }
}
