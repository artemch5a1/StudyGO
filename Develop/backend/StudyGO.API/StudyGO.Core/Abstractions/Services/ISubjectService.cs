using StudyGO.Contracts.Dtos.Subjects;

namespace StudyGO.Core.Abstractions.Services
{
    public interface ISubjectService
    {
        public Task<List<SubjectDto>> GetAllSubjects();

        public Task<SubjectDto> GetSubjectById();
    }
}
