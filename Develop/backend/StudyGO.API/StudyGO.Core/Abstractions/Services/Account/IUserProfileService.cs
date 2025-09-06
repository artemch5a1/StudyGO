using StudyGO.Contracts.Contracts;
using StudyGO.Contracts.Dtos.UserProfiles;
using StudyGO.Contracts.PaginationContract;
using StudyGO.Contracts.Result;

namespace StudyGO.Core.Abstractions.Services.Account
{
    public interface IUserProfileService
    {
        public Task<Result<UserProfileDto?>> TryGetUserProfileById(Guid userId, CancellationToken cancellationToken = default);

        public Task<Result<List<UserProfileDto>>> GetAllUserProfiles(CancellationToken cancellationToken = default, 
            Pagination? value = null);

        Task<Result<List<UserProfileDto>>> GetAllUserVerifiedProfiles(
            CancellationToken cancellationToken = default,
            Pagination? value = null
        );

        Task<Result<UserProfileDto?>> TryGetVerifiedUserProfileById(
            Guid userId,
            CancellationToken cancellationToken = default
        );
    }
}
