using StudyGO.Contracts.Dtos.TutorProfiles;
using StudyGO.Contracts.Result;

namespace StudyGO.Core.Abstractions.Services.Account
{
    public interface ITutorProfileService
    {
        public Task<Result<Guid>> TryRegistry(TutorProfileRegistrDto profile, CancellationToken cancellationToken = default);

        public Task<Result<Guid>> TryUpdateUserProfile(TutorProfileUpdateDto newProfile, CancellationToken cancellationToken = default);

        public Task<Result<TutorProfileDto?>> TryGetUserProfileById(Guid userId, CancellationToken cancellationToken = default);

        public Task<Result<List<TutorProfileDto>>> GetAllUserProfiles(CancellationToken cancellationToken = default);
    }
}
