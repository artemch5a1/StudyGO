using MediatR;
using StudyGO.Contracts.Dtos.UserProfiles;
using StudyGO.Contracts.Result;

namespace StudyGO.Application.UseCases.UserProfileUseCases.Commands.UpdateUser;

public record UpdateUserProfileCommand(UserProfileUpdateDto NewProfile) : IRequest<Result<Guid>>;