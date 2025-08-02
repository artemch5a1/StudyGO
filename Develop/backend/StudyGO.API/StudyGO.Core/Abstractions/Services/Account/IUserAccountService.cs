using StudyGO.Contracts.Contracts;
using StudyGO.Contracts.Dtos.Users;

namespace StudyGO.Core.Abstractions.Services.Account
{
    public interface IUserAccountService
    {
        public Task<UserLoginResponseDto> TryLogIn(UserLoginRequest userLogin);

        public Task<bool> TryDeleteAccount(Guid id);

        public Task<bool> TryUpdateAccount(UserUpdateDto user);

        public Task<bool> TryUpdateAccount(UserUpdateСredentialsDto user);

        public Task<List<UserDto>> TryGetAllAccount();

        public Task<UserDto?> TryGetAccountById(Guid id);
    }
}
