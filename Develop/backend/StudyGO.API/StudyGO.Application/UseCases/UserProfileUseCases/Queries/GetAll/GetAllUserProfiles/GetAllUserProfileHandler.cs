using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using StudyGO.Contracts.Dtos.UserProfiles;
using StudyGO.Contracts.Result;
using StudyGO.Core.Abstractions.Repositories;

namespace StudyGO.Application.UseCases.UserProfileUseCases.Queries.GetAll.GetAllUserProfiles;

public class GetAllUserProfileHandler : IRequestHandler<GetAllUserProfileQuery, Result<List<UserProfileDto>>>
{
    private readonly IUserProfileRepository _userRepository;

    private readonly IMapper _mapper;

    private readonly ILogger<GetAllUserProfileHandler> _logger;

    public GetAllUserProfileHandler(
        IUserProfileRepository userRepository, 
        IMapper mapper, 
        ILogger<GetAllUserProfileHandler> logger
        )
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<List<UserProfileDto>>> Handle(GetAllUserProfileQuery request, CancellationToken cancellationToken)
    {
        var value = request.Value;
        
        _logger.LogInformation("Получение всех профилей пользователей");
            
        var result = value == null ? await _userRepository.GetAll(cancellationToken) : await _userRepository.GetPages(value.Skip, value.Take,
            cancellationToken);
            
        _logger.LogDebug("Получено {Count} профилей пользователей", result.Value?.Count ?? 0);
            
        return result.MapDataTo(_mapper.Map<List<UserProfileDto>>);
    }
}