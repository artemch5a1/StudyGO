using AutoMapper;
using Microsoft.Extensions.Logging;
using StudyGO.Core.Abstractions.Utils;
using StudyGO.Contracts.Contracts;
using StudyGO.Contracts.Dtos.Users;
using StudyGO.Core.Abstractions.Repositories;
using StudyGO.Core.Abstractions.Services.Account;
using StudyGO.Core.Models;

namespace StudyGO.Application.Services.Account
{
    public class UserAccountService : IUserAccountService
    {
        private readonly IUserRepository _userRepository;

        private readonly IMapper _mapper;

        private readonly ILogger _logger;

        private readonly IPasswordHasher _passwordHasher;

        public UserAccountService(
            IUserRepository userRepository,
            IMapper mapper,
            ILogger logger,
            IPasswordHasher passwordHasher
        )
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _logger = logger;
            _passwordHasher = passwordHasher;
        }

        public async Task<bool> TryDeleteAccount(Guid id)
        {
            return await _userRepository.Delete(id);
        }

        public async Task<UserDto?> TryGetAccountById(Guid id)
        {
            User? user = await _userRepository.GetById(id);

            return _mapper.Map<UserDto?>(user);
        }

        public async Task<List<UserDto>> TryGetAllAccount()
        {
            List<User> users = await _userRepository.GetAll();

            return _mapper.Map<List<UserDto>>(users);
        }

        public async Task<(string token, string? error)> TryLogIn(UserLoginRequest userLogin)
        {
            UserLoginResponse dbSearchCred = await _userRepository.GetCredentialByEmail(
                userLogin.Email
            );

            throw new NotImplementedException();
        }

        public async Task<bool> TryUpdateAccount(UserUpdateDto user)
        {
            return await TryUpdate(user);
        }

        public async Task<bool> TryUpdateAccount(UserUpdateСredentialsDto user)
        {
            return await TryUpdate(user);
        }

        private async Task<bool> TryUpdate<T>(T user)
        {
            User userModel = _mapper.Map<User>(user);
            return await _userRepository.Update(userModel);
        }
    }
}
