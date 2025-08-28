using MediatR;
using StudyGO.Contracts.Result;

namespace StudyGO.Application.UseCases.Commands.DeleteCommands;

public record DeleteAccountCommand(Guid Id) : IRequest<Result<Guid>>;