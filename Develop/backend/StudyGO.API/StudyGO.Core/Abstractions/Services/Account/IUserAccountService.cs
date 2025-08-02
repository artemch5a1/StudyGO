using StudyGO.Contracts.Contracts;
using StudyGO.Contracts.Dtos.Users;
using StudyGO.Contracts.Result;

namespace StudyGO.Core.Abstractions.Services.Account
{
    public interface IUserAccountService
    {
        public Task<Result<UserLoginResponseDto>> TryLogIn(UserLoginRequest userLogin);

        public Task<Result<Guid>> TryDeleteAccount(Guid id);

        public Task<Result<Guid>> TryUpdateAccount(UserUpdateDto user);

        public Task<Result<Guid>> TryUpdateAccount(UserUpdateСredentialsDto user);

        public Task<Result<List<UserDto>>> TryGetAllAccount();

        public Task<Result<UserDto?>> TryGetAccountById(Guid id);
    }
}
