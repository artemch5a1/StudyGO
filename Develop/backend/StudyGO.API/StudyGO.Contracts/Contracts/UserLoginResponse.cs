namespace StudyGO.Contracts.Contracts
{
    public record UserLoginResponse
    {
        public Guid? id { get; set; }
        public string Email { get; set; } = null!;

        public string Password { get; set; } = null!;

        public string? Role { get; set; }
    }
}
