using MediatR;
using StudyGO.Contracts.Result;
using StudyGO.Core.Abstractions.Repositories;

namespace StudyGO.Application.UseCases.Commands.SpecificCommands.ConfirmEmail;

public class ConfirmEmailHandler : IRequestHandler<ConfirmEmailCommand, Result<Guid>>
{
    private readonly IUserRepository _userRepository;

    public ConfirmEmailHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<Guid>> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        return await _userRepository.ConfirmEmailAsync(request.Contract.UserId, request.Contract.Token, cancellationToken);
    }
}