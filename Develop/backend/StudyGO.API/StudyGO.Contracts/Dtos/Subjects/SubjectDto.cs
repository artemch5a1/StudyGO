namespace StudyGO.Contracts.Dtos.Subjects
{
    public class SubjectDto 
    {
        public Guid SubjectId { get; set; }

        public string Title { get; set; } = null!;

        public string Description { get; set; } = string.Empty;
    }
}
