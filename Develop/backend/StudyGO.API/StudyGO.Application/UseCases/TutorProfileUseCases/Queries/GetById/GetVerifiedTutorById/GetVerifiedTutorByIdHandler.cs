using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using StudyGO.Application.Services.Account;
using StudyGO.Contracts.Dtos.TutorProfiles;
using StudyGO.Contracts.Result;
using StudyGO.Core.Abstractions.Repositories;
using StudyGO.Core.Extensions;

namespace StudyGO.Application.UseCases.TutorProfileUseCases.Queries.GetById.GetVerifiedTutorById;

public class GetVerifiedTutorByIdHandler : IRequestHandler<GetVerifiedTutorByIdQuery, Result<TutorProfileDto?>>
{
    private readonly ITutorProfileRepository _userRepository;

    private readonly IMapper _mapper;

    private readonly ILogger<GetVerifiedTutorByIdHandler> _logger;

    public GetVerifiedTutorByIdHandler(
        ITutorProfileRepository userRepository, 
        IMapper mapper, 
        ILogger<GetVerifiedTutorByIdHandler> logger)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _logger = logger;
    }
    
    public async Task<Result<TutorProfileDto?>> Handle(GetVerifiedTutorByIdQuery request, CancellationToken cancellationToken)
    {
        var userId = request.UserId;
        
        _logger.LogInformation("Поиск профиля подтвержденного учителя по ID: {UserId}", userId);
            
        var result = await _userRepository.GetByIdVerified(userId, cancellationToken);
            
        _logger.LogResult(result, 
            "Профиль учителя найден", 
            "Профиль учителя не найден", 
            new { UserId = userId });
            
        return result.MapDataTo(_mapper.Map<TutorProfileDto?>);
    }
}