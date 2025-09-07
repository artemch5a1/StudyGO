using MediatR;
using StudyGO.Contracts.Dtos.Users;
using StudyGO.Contracts.Result;

namespace StudyGO.Application.UseCases.UserUseCases.Queries.GetById.GetAccountById;

public record GetAccountByIdQuery(Guid Id) : IRequest<Result<UserDto?>>;