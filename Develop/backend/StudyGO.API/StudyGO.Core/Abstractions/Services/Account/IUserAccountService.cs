using StudyGO.Contracts.Contracts;
using StudyGO.Contracts.Dtos.Users;
using StudyGO.Core.Models;

namespace StudyGO.Core.Abstractions.Services.Account
{
    public interface IUserAccountService
    {
        public Task<UserLoginResponse> TryLogIn(UserLoginRequest userLogin);

        public Task<bool> TryDeleteAccount(Guid id);

        public Task<bool> TryUpdateAccount(UserUpdateDto user);

        public Task<bool> TryUpdateAccount(UserUpdateСredentialsDto user);
    }
}
