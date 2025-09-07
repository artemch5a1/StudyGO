using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using StudyGO.Contracts.Dtos.Subjects;
using StudyGO.Contracts.Result;
using StudyGO.Core.Abstractions.Repositories;

namespace StudyGO.Application.UseCases.CatalogUseCases.SubjectUseCases.GetAllSubjects;

public class GetAllSubjectsHandler : IRequestHandler<GetAllSubjectsQuery, Result<List<SubjectDto>>>
{
    private readonly ISubjectRepository _subjectRepository;

    private readonly IMapper _mapper;

    private readonly ILogger<GetAllSubjectsHandler> _logger;

    public GetAllSubjectsHandler(
        ISubjectRepository subjectRepository,
        IMapper mapper,
        ILogger<GetAllSubjectsHandler> logger
    )
    {
        _subjectRepository = subjectRepository;
        _mapper = mapper;
        _logger = logger;
    }
    
    public async Task<Result<List<SubjectDto>>> Handle(GetAllSubjectsQuery request, CancellationToken cancellationToken)
    {
        var result = await _subjectRepository.GetAll(cancellationToken);
            
        _logger.LogDebug("Отправлен запрос на получение предметов");
            
        return result.MapDataTo(_mapper.Map<List<SubjectDto>>);
    }
}