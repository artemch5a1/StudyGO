using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using StudyGO.Contracts.Dtos.TutorProfiles;
using StudyGO.Contracts.Result;
using StudyGO.Core.Abstractions.Repositories;

namespace StudyGO.Application.UseCases.TutorProfileUseCases.Queries.GetAll.GetAllVerifiedTutors;

public class GetAllVerifiedTutorHandler : IRequestHandler<GetAllVerifiedTutorQuery, Result<List<TutorProfileDto>>>
{
    private readonly ITutorProfileRepository _userRepository;

    private readonly IMapper _mapper;

    private readonly ILogger<GetAllVerifiedTutorHandler> _logger;

    public GetAllVerifiedTutorHandler(
        ITutorProfileRepository userRepository, 
        IMapper mapper, 
        ILogger<GetAllVerifiedTutorHandler> logger)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _logger = logger;
    }
    
    public async Task<Result<List<TutorProfileDto>>> Handle(GetAllVerifiedTutorQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Получение всех подтвержденных профилей учителей");
        
        var value = request.Value;
        
        var result = value == null ? await _userRepository.GetAllVerified(cancellationToken) : 
            await _userRepository.GetPagesVerified(value.Skip, value.Take, cancellationToken);
            
        _logger.LogDebug("Получено {Count} профилей учителей", result.Value?.Count ?? 0);
            
        return result.MapDataTo(_mapper.Map<List<TutorProfileDto>>);
    }
}