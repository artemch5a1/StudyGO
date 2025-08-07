using StudyGO.Contracts.Dtos.UserProfiles;
using StudyGO.Contracts.Result;

namespace StudyGO.Core.Abstractions.Services.Account
{
    public interface IUserProfileService
    {
        public Task<Result<Guid>> TryRegistr(UserProfileRegistrDto profile, CancellationToken cancellationToken = default);

        public Task<Result<Guid>> TryUpdateUserProfile(UserProfileUpdateDto newProfile, CancellationToken cancellationToken = default);

        public Task<Result<UserProfileDto?>> TryGetUserProfileById(Guid userId, CancellationToken cancellationToken = default);

        public Task<Result<List<UserProfileDto>>> GetAllUserProfiles(CancellationToken cancellationToken = default);
    }
}
