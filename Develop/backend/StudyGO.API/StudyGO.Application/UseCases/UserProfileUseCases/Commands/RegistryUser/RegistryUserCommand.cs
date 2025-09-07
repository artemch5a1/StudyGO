using MediatR;
using StudyGO.Contracts.Contracts;
using StudyGO.Contracts.Dtos.UserProfiles;
using StudyGO.Contracts.Result;

namespace StudyGO.Application.UseCases.UserProfileUseCases.Commands.RegistryUser;

public record RegistryUserCommand(UserProfileRegistrDto Profile, string ConfirmEmailEndpoint) 
    : IRequest<Result<UserRegistryResponse>>;