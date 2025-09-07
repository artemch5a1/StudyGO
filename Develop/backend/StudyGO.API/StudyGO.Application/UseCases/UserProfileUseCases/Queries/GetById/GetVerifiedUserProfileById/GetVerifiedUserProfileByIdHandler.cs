using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using StudyGO.Contracts.Dtos.UserProfiles;
using StudyGO.Contracts.Result;
using StudyGO.Core.Abstractions.Repositories;
using StudyGO.Core.Extensions;

namespace StudyGO.Application.UseCases.UserProfileUseCases.Queries.GetById.GetVerifiedUserProfileById;

public class GetVerifiedUserProfileByIdHandler : IRequestHandler<GetVerifiedUserProfileByIdQuery, Result<UserProfileDto?>>
{
    private readonly IUserProfileRepository _userRepository;

    private readonly IMapper _mapper;

    private readonly ILogger<GetVerifiedUserProfileByIdHandler> _logger;

    public GetVerifiedUserProfileByIdHandler(
        IUserProfileRepository userRepository, 
        IMapper mapper, 
        ILogger<GetVerifiedUserProfileByIdHandler> logger
        )
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<UserProfileDto?>> Handle(GetVerifiedUserProfileByIdQuery request, CancellationToken cancellationToken)
    {
        var userId = request.Id;
        
        _logger.LogInformation("Поиск профиля пользователя по ID: {UserId}", userId);
            
        var result = await _userRepository.GetByIdVerified(userId, cancellationToken);
            
        _logger.LogResult(result, 
            "Подтвержденный профиль пользователя найден", 
            "Профиль пользователя не найден среди подтвержденных", 
            new { UserId = userId });
            
        return result.MapDataTo(_mapper.Map<UserProfileDto?>);
    }
}