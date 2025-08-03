using AutoMapper;
using Microsoft.Extensions.Logging;
using StudyGO.Contracts.Dtos.Subjects;
using StudyGO.Contracts.Result;
using StudyGO.Core.Abstractions.Repositories;
using StudyGO.Core.Abstractions.Services;

namespace StudyGO.Application.Services
{
    public class SubjectService : ISubjectService
    {
        private readonly ISubjectRepository _subjectRepository;

        private readonly IMapper _mapper;

        private readonly ILogger<SubjectService> _logger;

        public SubjectService(
            ISubjectRepository subjectRepository,
            IMapper mapper,
            ILogger<SubjectService> logger
        )
        {
            _subjectRepository = subjectRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<List<SubjectDto>>> GetAllSubjects()
        {
            var result = await _subjectRepository.GetAll();

            return result.MapTo(_mapper.Map<List<SubjectDto>>);
        }

        public async Task<Result<SubjectDto?>> GetSubjectById(Guid id)
        {
            var result = await _subjectRepository.GetById(id);

            return result.MapTo(_mapper.Map<SubjectDto?>);
        }
    }
}
