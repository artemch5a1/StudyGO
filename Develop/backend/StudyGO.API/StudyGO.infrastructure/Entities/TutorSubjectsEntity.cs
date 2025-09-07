using System.ComponentModel.DataAnnotations.Schema;

namespace StudyGO.infrastructure.Entities;

public class TutorSubjectsEntity
{
    public Guid TutorId { get; set; }
    
    [ForeignKey(nameof(TutorId))]
    public virtual TutorProfileEntity? Tutor { get; set; } = null!;

    public Guid SubjectId { get; set; }
    
    [ForeignKey(nameof(SubjectId))]
    public virtual SubjectEntity? Subject { get; set; } = null!;
}