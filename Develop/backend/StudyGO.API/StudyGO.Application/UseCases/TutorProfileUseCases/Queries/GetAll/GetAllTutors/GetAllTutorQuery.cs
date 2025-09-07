using MediatR;
using StudyGO.Contracts.Dtos.TutorProfiles;
using StudyGO.Contracts.PaginationContract;
using StudyGO.Contracts.Result;

namespace StudyGO.Application.UseCases.TutorProfileUseCases.Queries.GetAll.GetAllTutors;

public record GetAllTutorQuery(Pagination? Value = null) : IRequest<Result<List<TutorProfileDto>>>;