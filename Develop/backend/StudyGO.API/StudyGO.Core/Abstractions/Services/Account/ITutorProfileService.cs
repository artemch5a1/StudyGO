using StudyGO.Contracts.Dtos.TutorProfiles;
using StudyGO.Contracts.PaginationContract;
using StudyGO.Contracts.Result;

namespace StudyGO.Core.Abstractions.Services.Account
{
    public interface ITutorProfileService
    {
        public Task<Result<Guid>> TryRegistry(TutorProfileRegistrDto profile, string confirmEmailEndpoint, CancellationToken cancellationToken = default);

        public Task<Result<Guid>> TryUpdateUserProfile(TutorProfileUpdateDto newProfile, CancellationToken cancellationToken = default);

        public Task<Result<TutorProfileDto?>> TryGetUserProfileById(Guid userId, CancellationToken cancellationToken = default);

        public Task<Result<List<TutorProfileDto>>> GetAllUserProfiles(CancellationToken cancellationToken = default, 
            Pagination? value = null);
        
        Task<Result<List<TutorProfileDto>>> GetAllUserVerifiedProfiles(
            CancellationToken cancellationToken = default,
            Pagination? value = null
        );

        Task<Result<TutorProfileDto?>> TryGetVerifiedUserProfileById(
            Guid userId,
            CancellationToken cancellationToken = default
        );
    }
}
