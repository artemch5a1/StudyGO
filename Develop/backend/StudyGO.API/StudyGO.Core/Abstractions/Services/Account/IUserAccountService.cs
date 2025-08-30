using StudyGO.Contracts.Contracts;
using StudyGO.Contracts.Dtos.Users;
using StudyGO.Contracts.Result;

namespace StudyGO.Core.Abstractions.Services.Account
{
    public interface IUserAccountService
    {
        public Task<Result<UserLoginResponseDto>> TryLogIn(UserLoginRequest userLogin, CancellationToken cancellationToken = default);

        public Task<Result<Guid>> TryDeleteAccount(Guid id, CancellationToken cancellationToken = default);

        public Task<Result<Guid>> TryUpdateAccount(UserUpdateDto user, CancellationToken cancellationToken = default);

        public Task<Result<List<UserDto>>> TryGetAllAccount(CancellationToken cancellationToken = default);

        public Task<Result<UserDto?>> TryGetAccountById(Guid id, CancellationToken cancellationToken = default);

        public Task<Result<Guid>> ConfirmEmailAsync(Guid userId, string userToken,
            CancellationToken cancellationToken = default);
    }
}
