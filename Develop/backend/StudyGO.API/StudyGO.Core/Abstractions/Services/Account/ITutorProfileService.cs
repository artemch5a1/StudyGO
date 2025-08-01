using StudyGO.Contracts.Dtos.TutorProfiles;

namespace StudyGO.Core.Abstractions.Services.Account
{
    public interface ITutorProfileService
    {
        public Task<Guid> TryRegistr(TutorProfileRegistrDto profile);

        public Task<bool> TryUpdateUserProfile(TutorProfileUpdateDto newProfile);

        public Task<TutorProfileDto?> TryGetUserProfileById(Guid userId);

        public Task<List<TutorProfileDto>> GetAllUserProfiles();
    }
}
