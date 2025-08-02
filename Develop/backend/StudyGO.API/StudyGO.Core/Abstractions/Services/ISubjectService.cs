using StudyGO.Contracts.Dtos.Subjects;
using StudyGO.Contracts.Result;

namespace StudyGO.Core.Abstractions.Services
{
    public interface ISubjectService
    {
        public Task<Result<List<SubjectDto>>> GetAllSubjects();

        public Task<Result<SubjectDto?>> GetSubjectById();
    }
}
