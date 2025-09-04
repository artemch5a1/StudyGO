using MediatR;
using StudyGO.Contracts.Dtos.TutorProfiles;
using StudyGO.Contracts.Result;

namespace StudyGO.Application.UseCases.TutorProfileUseCases.Commands.UpdateTutor;

public record UpdateTutorCommand(TutorProfileUpdateDto NewProfile) : IRequest<Result<Guid>>;