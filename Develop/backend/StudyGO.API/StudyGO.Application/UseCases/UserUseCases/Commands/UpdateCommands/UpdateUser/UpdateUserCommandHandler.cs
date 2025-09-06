using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using StudyGO.Contracts.Result;
using StudyGO.Core.Abstractions.Repositories;
using StudyGO.Core.Models;

namespace StudyGO.Application.UseCases.UserUseCases.Commands.UpdateCommands.UpdateUser;

public class UpdateUserCommandHandler
    : IRequestHandler<UpdateUserCommand, Result<Guid>>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateUserCommandHandler> _logger;
    
    public UpdateUserCommandHandler(
        IUserRepository userRepository,
        IMapper mapper,
        ILogger<UpdateUserCommandHandler> logger)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _logger = logger;
    }
    
    public async Task<Result<Guid>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var userDto = request.User;

        _logger.LogInformation("Обновление данных аккаунта: {UserId}", userDto.UserId);
        
        var userModel = _mapper.Map<User>(userDto);

        _logger.LogDebug("Отправлен запрос в репозиторий");

        return await _userRepository.Update(userModel, cancellationToken);
    }
}