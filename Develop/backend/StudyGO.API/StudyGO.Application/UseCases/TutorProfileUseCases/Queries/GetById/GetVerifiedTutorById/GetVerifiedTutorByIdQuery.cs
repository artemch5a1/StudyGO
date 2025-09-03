using MediatR;
using StudyGO.Contracts.Dtos.TutorProfiles;
using StudyGO.Contracts.Result;

namespace StudyGO.Application.UseCases.TutorProfileUseCases.Queries.GetById.GetVerifiedTutorById;

public record GetVerifiedTutorByIdQuery(Guid UserId) : IRequest<Result<TutorProfileDto?>>;