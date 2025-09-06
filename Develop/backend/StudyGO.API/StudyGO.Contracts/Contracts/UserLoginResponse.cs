namespace StudyGO.Contracts.Contracts
{
    public record UserLoginResponse
    {
        public Guid Id { get; set; }

        public string Email { get; set; } = null!;

        public string PasswordHash { get; set; } = null!;

        public string Role { get; set; } = string.Empty;
    }
}
