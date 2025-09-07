using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using StudyGO.Contracts.Result;
using StudyGO.Core.Abstractions.Repositories;
using StudyGO.Core.Models;

namespace StudyGO.Application.UseCases.TutorProfileUseCases.Commands.UpdateTutor;

public class UpdateTutorHandler : IRequestHandler<UpdateTutorCommand, Result<Guid>>
{
    private readonly ITutorProfileRepository _userRepository;

    private readonly IMapper _mapper;

    private readonly ILogger<UpdateTutorHandler> _logger;

    public UpdateTutorHandler(
        ITutorProfileRepository userRepository, 
        IMapper mapper, 
        ILogger<UpdateTutorHandler> logger
        )
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(UpdateTutorCommand request, CancellationToken cancellationToken)
    {
        var newProfile = request.NewProfile;
        
        _logger.LogInformation("Обновление профиля учителя: {UserId}", newProfile.UserId);
            
        _logger.LogDebug("Отправлен запрос в репозиторий");
            
        return await _userRepository.Update(
            _mapper.Map<TutorProfile>(newProfile),
            cancellationToken
        );
    }
}