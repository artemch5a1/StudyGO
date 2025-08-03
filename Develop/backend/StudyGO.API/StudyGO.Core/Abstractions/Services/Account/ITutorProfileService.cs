using StudyGO.Contracts.Dtos.TutorProfiles;
using StudyGO.Contracts.Result;

namespace StudyGO.Core.Abstractions.Services.Account
{
    public interface ITutorProfileService
    {
        public Task<Result<Guid>> TryRegistr(TutorProfileRegistrDto profile);

        public Task<Result<Guid>> TryUpdateUserProfile(TutorProfileUpdateDto newProfile);

        public Task<Result<TutorProfileDto?>> TryGetUserProfileById(Guid userId);

        public Task<Result<List<TutorProfileDto>>> GetAllUserProfiles();
    }
}
