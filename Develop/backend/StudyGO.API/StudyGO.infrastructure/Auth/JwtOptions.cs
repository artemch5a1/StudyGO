namespace StudyGO.infrastructure.Auth
{
    public class JwtOptions
    {
        public string Key { get; set; } = null!;

        public string Issuer { get; set; } = null!;

        public int ExpiresMinutes { get; set; } = 120;
    }
}
