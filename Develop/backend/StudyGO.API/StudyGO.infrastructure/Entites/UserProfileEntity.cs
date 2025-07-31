namespace StudyGO.infrastructure.Entites
{
    public class UserProfileEntity
    {
        public Guid UserID { get; set; }

        public virtual UserEntity User { get; set; } = null!;

        public DateTime DateBirth { get; set; }

        public Guid SubjectID { get; set; }

        public virtual SubjectEntity? FavoriteSubject { get; set; } = null;

        public string Description { get; set; } = null!;
    }
}
