namespace StudyGO.Contracts.Dtos.Users
{
    public class UserUpdateDto
    {
        public Guid UserID { get; set; }

        public string Surname { get; set; } = null!;

        public string Name { get; set; } = null!;

        public string Patronymic { get; set; } = null!;

        public string Number { get; set; } = string.Empty;
    }
}
