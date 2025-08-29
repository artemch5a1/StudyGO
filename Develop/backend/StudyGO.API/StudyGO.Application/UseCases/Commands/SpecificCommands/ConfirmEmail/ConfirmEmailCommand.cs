using MediatR;
using StudyGO.Contracts.Contracts;
using StudyGO.Contracts.Result;

namespace StudyGO.Application.UseCases.Commands.SpecificCommands.ConfirmEmail;

public record ConfirmEmailCommand(ConfirmEmailRequest Contract) : IRequest<Result<Guid>>;