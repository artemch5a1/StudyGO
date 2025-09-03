using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using StudyGO.Application.Services.Account;
using StudyGO.Contracts.Dtos.TutorProfiles;
using StudyGO.Contracts.Result;
using StudyGO.Core.Abstractions.Repositories;

namespace StudyGO.Application.UseCases.TutorProfileUseCases.Queries.GetAll.GetAllTutors;

public class GetAllTutorHandler : IRequestHandler<GetAllTutorQuery, Result<List<TutorProfileDto>>>
{
    private readonly ITutorProfileRepository _userRepository;

    private readonly IMapper _mapper;

    private readonly ILogger<TutorProfileService> _logger;

    public GetAllTutorHandler(
        ITutorProfileRepository userRepository, 
        IMapper mapper, 
        ILogger<TutorProfileService> logger)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<List<TutorProfileDto>>> Handle(GetAllTutorQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Получение всех профилей учителей");

        var value = request.Value;
        
        var result = value == null ? await _userRepository.GetAll(cancellationToken) : 
            await _userRepository.GetPages(value.Skip, value.Take, cancellationToken);
            
        _logger.LogDebug("Получено {Count} профилей учителей", result.Value?.Count ?? 0);
            
        return result.MapDataTo(_mapper.Map<List<TutorProfileDto>>);
    }
}