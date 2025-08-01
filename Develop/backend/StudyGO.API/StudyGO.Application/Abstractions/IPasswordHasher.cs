namespace StudyGO.Application.Abstractions
{
    public interface IPasswordHasher
    {
        public string GeneratePasswordHash(string password);

        public bool VerifiyPassword(string password, string passwordHash);
    }
}
