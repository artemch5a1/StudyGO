using MediatR;
using StudyGO.Contracts.Contracts;
using StudyGO.Contracts.Dtos.Users;
using StudyGO.Contracts.Result;

namespace StudyGO.Application.UseCases.Commands.SpecificCommands.LogInUser;

public record LogInUserCommand(UserLoginRequest UserLogin) : IRequest<Result<UserLoginResponseDto>>;