using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using StudyGO.Contracts.Result;
using StudyGO.Core.Abstractions.Repositories;
using StudyGO.Core.Models;

namespace StudyGO.Application.UseCases.UserProfileUseCases.Commands.UpdateUser;

public class UpdateUserProfileHandler : IRequestHandler<UpdateUserProfileCommand, Result<Guid>>
{
    private readonly IUserProfileRepository _userRepository;

    private readonly IMapper _mapper;

    private readonly ILogger<UpdateUserProfileHandler> _logger;

    public UpdateUserProfileHandler(
        IUserProfileRepository userRepository, 
        IMapper mapper, 
        ILogger<UpdateUserProfileHandler> logger
        )
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
    {
        var newProfile = request.NewProfile;
        
        _logger.LogInformation("Обновление профиля пользователя: {UserId}", newProfile.UserId);
        
        UserProfile user = _mapper.Map<UserProfile>(newProfile);
            
        _logger.LogDebug("Отправлен запрос в репозиторий");
            
        return await _userRepository.Update(user, cancellationToken);
    }
}