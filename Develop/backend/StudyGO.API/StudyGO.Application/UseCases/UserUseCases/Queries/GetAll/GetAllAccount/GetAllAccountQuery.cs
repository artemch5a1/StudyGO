using MediatR;
using StudyGO.Contracts.Dtos.Users;
using StudyGO.Contracts.Result;

namespace StudyGO.Application.UseCases.UserUseCases.Queries.GetAll.GetAllAccount;

public record GetAllAccountQuery() : IRequest<Result<List<UserDto>>>;