namespace StudyGO.Contracts.Contracts
{
    public record UserLoginRequest
    {
        public string Email { get; set; } = null!;

        public string Password { get; set; } = null!;

        public string? Role { get; set; }
        public Guid? id { get; set; }
    }
}
