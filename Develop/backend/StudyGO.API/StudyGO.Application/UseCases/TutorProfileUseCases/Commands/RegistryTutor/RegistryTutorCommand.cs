using MediatR;
using StudyGO.Contracts.Contracts;
using StudyGO.Contracts.Dtos.TutorProfiles;
using StudyGO.Contracts.Result;

namespace StudyGO.Application.UseCases.TutorProfileUseCases.Commands.RegistryTutor;

public record RegistryTutorCommand(TutorProfileRegistrDto Profile, string ConfirmEmailEndpoint) 
    : IRequest<Result<UserRegistryResponse>>;