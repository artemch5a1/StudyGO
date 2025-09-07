using MediatR;
using StudyGO.Contracts.Dtos.UserProfiles;
using StudyGO.Contracts.PaginationContract;
using StudyGO.Contracts.Result;

namespace StudyGO.Application.UseCases.UserProfileUseCases.Queries.GetAll.GetAllUserProfiles;

public record GetAllUserProfileQuery(Pagination? Value = null) 
    : IRequest<Result<List<UserProfileDto>>>;