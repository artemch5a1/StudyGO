using MediatR;
using StudyGO.Contracts.Result;

namespace StudyGO.Application.UseCases.UserUseCases.Commands.DeleteCommands.DeleteAccount;

public record DeleteAccountCommand(Guid Id) : IRequest<Result<Guid>>;