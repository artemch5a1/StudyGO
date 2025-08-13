using StudyGO.Core.Enums;

namespace StudyGO.Core.Models
{
    public class UserProfile
    {
        private UserProfile(
            Guid userId, 
            DateOnly dateBirth, 
            Guid? subjectId,
            string description
        )
        {
            UserId = userId;
            DateBirth = dateBirth;
            SubjectId = subjectId;
            Description = description;
        }
        
        private UserProfile(
            User user, 
            DateOnly dateBirth, 
            Guid? subjectId,
            string description
            )
        {
            User = user;
            DateBirth = dateBirth;
            SubjectId = subjectId;
            Description = description;
        }
        
        public Guid UserId { get; set; }

        public User? User { get; set; }

        public DateOnly DateBirth { get; set; }

        public Guid? SubjectId { get; set; }

        public Subject? FavoriteSubject { get; set; } = null;

        public string Description { get; set; } = null!;

        public static UserProfile CreateUser(
            string email,
            string passwordHash,
            string surname,
            string name,
            string patronymic,
            string? number,
            DateOnly dateBirth, 
            Guid? subjectId,
            string description
            )
        {
            var user = User.CreateUser(email, passwordHash, surname, name, patronymic, number, RolesEnum.User);
            
            return new UserProfile(user, dateBirth, subjectId, description);
        }

        public static UserProfile UpdateUser(
            Guid userId,
            DateOnly dateBirth,
            Guid? subjectId,
            string description
        )
        {
            return new UserProfile(userId, dateBirth, subjectId, description);
        }
    }
}
