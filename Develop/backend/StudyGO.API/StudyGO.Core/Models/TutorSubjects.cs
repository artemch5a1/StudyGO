namespace StudyGO.Core.Models;

public class TutorSubjects
{
    public Guid TutorId { get; set; }

    public virtual TutorProfile? Tutor { get; set; } = null!;

    public Guid SubjectId { get; set; }

    public virtual Subject? Subject { get; set; } = null!;
}