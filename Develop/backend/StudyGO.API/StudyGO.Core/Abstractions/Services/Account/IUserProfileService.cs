using StudyGO.Contracts.Dtos.UserProfiles;

namespace StudyGO.Core.Abstractions.Services.Account
{
    public interface IUserProfileService
    {
        public Task<Guid> TryRegistr(UserProfileRegistrDto profile);

        public Task<bool> TryUpdateUserProfile(UserProfileUpdateDto newProfile);

        public Task<UserProfileDto?> TryGetUserProfileById(Guid userId);

        public Task<List<UserProfileDto>> GetAllUserProfiles();
    }
}
