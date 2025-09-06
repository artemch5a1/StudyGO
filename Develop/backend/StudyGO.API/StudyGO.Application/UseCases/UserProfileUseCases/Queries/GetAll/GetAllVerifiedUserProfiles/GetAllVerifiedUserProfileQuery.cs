using MediatR;
using StudyGO.Contracts.Dtos.UserProfiles;
using StudyGO.Contracts.PaginationContract;
using StudyGO.Contracts.Result;

namespace StudyGO.Application.UseCases.UserProfileUseCases.Queries.GetAll.GetAllVerifiedUserProfiles;

public record GetAllVerifiedUserProfileQuery(Pagination? Value = null) 
    : IRequest<Result<List<UserProfileDto>>>;