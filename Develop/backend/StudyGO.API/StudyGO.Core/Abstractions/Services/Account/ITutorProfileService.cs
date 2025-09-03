using StudyGO.Contracts.Contracts;
using StudyGO.Contracts.Dtos.TutorProfiles;
using StudyGO.Contracts.Result;

namespace StudyGO.Core.Abstractions.Services.Account
{
    public interface ITutorProfileService
    {
        public Task<Result<UserRegistryResponse>> TryRegistry(TutorProfileRegistrDto profile, string confirmEmailEndpoint, CancellationToken cancellationToken = default);

        public Task<Result<Guid>> TryUpdateUserProfile(TutorProfileUpdateDto newProfile, CancellationToken cancellationToken = default);
    }
}
