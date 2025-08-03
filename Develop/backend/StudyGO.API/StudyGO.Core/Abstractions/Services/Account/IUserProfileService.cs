using StudyGO.Contracts.Dtos.UserProfiles;
using StudyGO.Contracts.Result;

namespace StudyGO.Core.Abstractions.Services.Account
{
    public interface IUserProfileService
    {
        public Task<Result<Guid>> TryRegistr(UserProfileRegistrDto profile);

        public Task<Result<Guid>> TryUpdateUserProfile(UserProfileUpdateDto newProfile);

        public Task<Result<UserProfileDto?>> TryGetUserProfileById(Guid userId);

        public Task<Result<List<UserProfileDto>>> GetAllUserProfiles();
    }
}
