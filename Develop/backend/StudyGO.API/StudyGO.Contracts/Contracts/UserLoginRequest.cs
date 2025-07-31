namespace StudyGO.Contracts.Contracts
{
    public record UserLoginRequest
    {
        public string Email { get; set; } = null!;

        public string PasswordHash { get; set; } = null!;
    }
}
