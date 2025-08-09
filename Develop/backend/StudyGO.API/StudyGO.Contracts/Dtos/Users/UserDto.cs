namespace StudyGO.Contracts.Dtos.Users
{
    public class UserDto 
    {
        public Guid UserId { get; set; }
        public string Email { get; set; } = null!;

        public string Surname { get; set; } = null!;

        public string Name { get; set; } = null!;

        public string Patronymic { get; set; } = null!;

        public string Number { get; set; } = string.Empty;
    }
}
