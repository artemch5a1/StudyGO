using MediatR;
using StudyGO.Contracts.Dtos.TutorProfiles;
using StudyGO.Contracts.Result;

namespace StudyGO.Application.UseCases.TutorProfileUseCases.Queries.GetById.GetTutorById;

public record GetTutorByIdQuery(Guid UserId) : IRequest<Result<TutorProfileDto?>>;