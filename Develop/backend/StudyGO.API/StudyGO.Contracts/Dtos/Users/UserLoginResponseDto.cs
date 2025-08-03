namespace StudyGO.Contracts.Dtos.Users
{
    public class UserLoginResponseDto 
    {
        public string Token { get; set; } = null!;

        public Guid Id { get; set; }
    }
}
