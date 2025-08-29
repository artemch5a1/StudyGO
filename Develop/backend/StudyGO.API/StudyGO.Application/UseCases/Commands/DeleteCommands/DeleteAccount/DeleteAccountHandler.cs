using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using StudyGO.Application.UseCases.Commands.UpdateCommands.UpdateUser;
using StudyGO.Contracts.Result;
using StudyGO.Core.Abstractions.Repositories;

namespace StudyGO.Application.UseCases.Commands.DeleteCommands.DeleteAccount;

public class DeleteAccountHandler : IRequestHandler<DeleteAccountCommand, Result<Guid>>
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UpdateUserCommandHandler> _logger;
    
    public DeleteAccountHandler(
        IUserRepository userRepository,
        ILogger<UpdateUserCommandHandler> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }
    
    public async Task<Result<Guid>> Handle(DeleteAccountCommand request, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Отправлен запрос на удаление пользователя {UserId}", request.Id);
        return await _userRepository.Delete(request.Id, cancellationToken);
    }
}