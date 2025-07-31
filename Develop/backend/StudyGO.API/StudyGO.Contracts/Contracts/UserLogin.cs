namespace StudyGO.Contracts.Contracts
{
    public record UserLogin
    {
        public string Email { get; set; } = null!;

        public string PasswordHash { get; set; } = null!;
    }
}
