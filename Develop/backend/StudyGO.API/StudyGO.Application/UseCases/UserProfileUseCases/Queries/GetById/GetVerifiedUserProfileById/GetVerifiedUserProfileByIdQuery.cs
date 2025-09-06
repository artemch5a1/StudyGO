using MediatR;
using StudyGO.Contracts.Dtos.UserProfiles;
using StudyGO.Contracts.Result;

namespace StudyGO.Application.UseCases.UserProfileUseCases.Queries.GetById.GetVerifiedUserProfileById;

public record GetVerifiedUserProfileByIdQuery(Guid Id) 
    : IRequest<Result<UserProfileDto?>>;