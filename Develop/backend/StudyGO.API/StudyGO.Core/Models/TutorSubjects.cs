namespace StudyGO.Core.Models;

public class TutorSubjects
{
    public Guid TutorId { get; set; }

    public TutorProfile? Tutor { get; set; } = null!;

    public Guid SubjectId { get; set; }

    public Subject? Subject { get; set; } = null!;
}