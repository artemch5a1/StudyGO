using MediatR;
using StudyGO.Contracts.Dtos.Users;
using StudyGO.Contracts.Result;

namespace StudyGO.Application.UseCases.UserUseCases.Commands.UpdateCommands.UpdateUserPassword;

public record UpdateUserPasswordCommand(UserUpdatePasswordDto User) : IRequest<Result<Guid>>;