using StudyGO.Contracts.Dtos.Subjects;
using StudyGO.Contracts.Result;

namespace StudyGO.Core.Abstractions.Services
{
    public interface ISubjectService
    {
        public Task<Result<List<SubjectDto>>> GetAllSubjects(CancellationToken cancellationToken = default);

        public Task<Result<SubjectDto?>> GetSubjectById(Guid id, CancellationToken cancellationToken = default);
    }
}
