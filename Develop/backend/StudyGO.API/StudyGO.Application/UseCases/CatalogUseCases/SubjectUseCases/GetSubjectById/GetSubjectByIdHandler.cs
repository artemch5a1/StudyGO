using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using StudyGO.Application.Services;
using StudyGO.Contracts.Dtos.Subjects;
using StudyGO.Contracts.Result;
using StudyGO.Core.Abstractions.Repositories;

namespace StudyGO.Application.UseCases.CatalogUseCases.SubjectUseCases.GetSubjectById;

public class GetSubjectByIdHandler : IRequestHandler<GetSubjectByIdQuery, Result<SubjectDto?>>
{
    private readonly ISubjectRepository _subjectRepository;

    private readonly IMapper _mapper;

    private readonly ILogger<GetSubjectByIdHandler> _logger;

    public GetSubjectByIdHandler(
        ISubjectRepository subjectRepository,
        IMapper mapper,
        ILogger<GetSubjectByIdHandler> logger
    )
    {
        _subjectRepository = subjectRepository;
        _mapper = mapper;
        _logger = logger;
    }
    
    public async Task<Result<SubjectDto?>> Handle(GetSubjectByIdQuery request, CancellationToken cancellationToken)
    {
        var id = request.Id;
        
        var result = await _subjectRepository.GetById(id, cancellationToken);
            
        _logger.LogDebug("Отправлен запрос на получение предмета по id {SubjectId}", id);
            
        return result.MapDataTo(_mapper.Map<SubjectDto?>);
    }
}