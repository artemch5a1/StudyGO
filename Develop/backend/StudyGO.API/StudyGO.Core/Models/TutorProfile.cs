using StudyGO.Core.Enums;

namespace StudyGO.Core.Models
{
    public class TutorProfile 
    {
        private TutorProfile(
            Guid userId,
            string bio, 
            decimal pricePerHour, 
            string city, 
            int formatId,
            ICollection<TutorSubjects> tutorSubjects
            )
        {
            UserId = userId;
            Bio = bio;
            PricePerHour = pricePerHour;
            City = city;
            FormatId = formatId;
            TutorSubjects = tutorSubjects;
        }

        private TutorProfile(
            string bio, 
            decimal pricePerHour, 
            string city, 
            int formatId,
            ICollection<TutorSubjects> tutorSubjects,
            User user)
        {
            User = user;
            Bio = bio;
            PricePerHour = pricePerHour;
            City = city;
            FormatId = formatId;
            TutorSubjects = tutorSubjects;
        }
        
        public Guid UserId { get; set; }

        public User? User { get; set; }

        public string Bio { get; set; } = string.Empty;

        public decimal PricePerHour { get; set; } = decimal.Zero;

        public string City { get; set; } = string.Empty;

        public int FormatId { get; set; }

        public Format? Format { get; set; }

        public ICollection<TutorSubjects> TutorSubjects { get; set; } = [];

        public static TutorProfile CreateTutor
        (
            string email,
            string passwordHash,
            string surname,
            string name,
            string patronymic,
            string? number,
            string bio, 
            decimal pricePerHour, 
            string city, 
            int formatId,
            ICollection<Guid> subjectsId)
        {
            User user = User.CreateUser(email, passwordHash, surname, name, patronymic, number, RolesEnum.Tutor);

            return new TutorProfile(bio, pricePerHour, city, formatId, subjectsId.Select(x => new TutorSubjects() 
            {
                SubjectId = x,
            }).ToList(), user);
        }

        public static TutorProfile UpdateTutor(
            Guid userId,
            string bio,
            decimal pricePerHour,
            string city,
            int formatId,
            ICollection<Guid> subjectsId)
        {
            return new TutorProfile(userId, bio, pricePerHour, city, formatId, subjectsId.Select(x =>
                new TutorSubjects()
                {
                    TutorId = userId,
                    SubjectId = x
                }).ToList());
        }
    }
}
