using MediatR;
using StudyGO.Contracts.Dtos.Users;
using StudyGO.Contracts.Result;

namespace StudyGO.Application.UseCases.Commands;

public record UpdateUserCommand(UserUpdateDto User) : IRequest<Result<Guid>>;