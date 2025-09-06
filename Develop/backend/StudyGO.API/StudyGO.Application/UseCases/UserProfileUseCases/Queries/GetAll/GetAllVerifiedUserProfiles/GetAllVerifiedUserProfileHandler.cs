using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using StudyGO.Contracts.Dtos.UserProfiles;
using StudyGO.Contracts.Result;
using StudyGO.Core.Abstractions.Repositories;

namespace StudyGO.Application.UseCases.UserProfileUseCases.Queries.GetAll.GetAllVerifiedUserProfiles;

public class GetAllVerifiedUserProfileHandler : 
    IRequestHandler<GetAllVerifiedUserProfileQuery, Result<List<UserProfileDto>>>
{
    private readonly IUserProfileRepository _userRepository;

    private readonly IMapper _mapper;

    private readonly ILogger<GetAllVerifiedUserProfileHandler> _logger;

    public GetAllVerifiedUserProfileHandler(
        IUserProfileRepository userRepository, 
        IMapper mapper, 
        ILogger<GetAllVerifiedUserProfileHandler> logger
        )
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<List<UserProfileDto>>> Handle(GetAllVerifiedUserProfileQuery request, CancellationToken cancellationToken)
    {
        var value = request.Value;
        
        _logger.LogInformation("Получение всех подтвержденных профилей пользователей");
            
        var result = value == null ? await _userRepository.GetAllVerified(cancellationToken) 
            : await _userRepository.GetPagesVerified(value.Skip, value.Take,
                cancellationToken);
            
        _logger.LogDebug("Получено {Count} подтвержденных профилей пользователей", result.Value?.Count ?? 0);
            
        return result.MapDataTo(_mapper.Map<List<UserProfileDto>>);
    }
}