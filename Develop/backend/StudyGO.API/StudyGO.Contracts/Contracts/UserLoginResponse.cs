namespace StudyGO.Contracts.Contracts
{
    public record UserLoginResponse
    {
        public bool IsLoggedIn { get; set; }
        public string Role { get; set; } = null!;
    }
}
