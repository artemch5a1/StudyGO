using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using StudyGO.Contracts.Dtos.UserProfiles;
using StudyGO.Contracts.Result;
using StudyGO.Core.Abstractions.Repositories;
using StudyGO.Core.Extensions;

namespace StudyGO.Application.UseCases.UserProfileUseCases.Queries.GetById.GetUserProfileById;

public class GetUserProfileByIdHandler : IRequestHandler<GetUserProfileByIdQuery, Result<UserProfileDto?>>
{
    private readonly IUserProfileRepository _userRepository;

    private readonly IMapper _mapper;

    private readonly ILogger<GetUserProfileByIdHandler> _logger;

    public GetUserProfileByIdHandler(
        IUserProfileRepository userRepository, 
        IMapper mapper, 
        ILogger<GetUserProfileByIdHandler> logger
        )
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<UserProfileDto?>> Handle(GetUserProfileByIdQuery request, CancellationToken cancellationToken)
    {
        var userId = request.Id;
        
        _logger.LogInformation("Поиск профиля пользователя по ID: {UserId}", userId);
            
        var result = await _userRepository.GetById(userId, cancellationToken);
            
        _logger.LogResult(result, 
            "Профиль пользователя найден", 
            "Профиль пользователя не найден", 
            new { UserId = userId });
            
        return result.MapDataTo(_mapper.Map<UserProfileDto?>);
    }
}