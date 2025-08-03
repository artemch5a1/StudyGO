namespace StudyGO.Contracts.Dtos.Users
{
    public class UserUpdateСredentialsDto
    {
        public Guid UserId { get; set; }

        public string Email { get; set; } = null!;

        public string Password { get; set; } = null!;

        public string OldPassword { get; set; } = null!;
    }
}
