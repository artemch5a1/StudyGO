using MediatR;
using StudyGO.Contracts.Dtos.UserProfiles;
using StudyGO.Contracts.Result;

namespace StudyGO.Application.UseCases.UserProfileUseCases.Queries.GetById.GetUserProfileById;

public record GetUserProfileByIdQuery(Guid Id) : IRequest<Result<UserProfileDto?>>;