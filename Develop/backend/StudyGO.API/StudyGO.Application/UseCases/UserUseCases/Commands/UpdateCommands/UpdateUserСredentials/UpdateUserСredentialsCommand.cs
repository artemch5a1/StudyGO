using MediatR;
using StudyGO.Contracts.Dtos.Users;
using StudyGO.Contracts.Result;

namespace StudyGO.Application.UseCases.UserUseCases.Commands.UpdateCommands.UpdateUserСredentials;

public record UpdateUserСredentialsCommand(UserUpdateСredentialsDto User) : IRequest<Result<Guid>>;