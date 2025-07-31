namespace StudyGO.Contracts.Dtos.Users
{
    public class UserUpdateСredentialsDto
    {
        public string Email { get; set; } = null!;

        public string PasswordHash { get; set; } = null!;
    }
}
