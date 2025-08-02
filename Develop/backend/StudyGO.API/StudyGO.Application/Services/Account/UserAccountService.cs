using AutoMapper;
using Microsoft.Extensions.Logging;
using StudyGO.Application.Extensions;
using StudyGO.Contracts.Contracts;
using StudyGO.Contracts.Dtos.Users;
using StudyGO.Contracts.Result;
using StudyGO.Core.Abstractions.Auth;
using StudyGO.Core.Abstractions.Repositories;
using StudyGO.Core.Abstractions.Services.Account;
using StudyGO.Core.Abstractions.Utils;
using StudyGO.Core.Models;

namespace StudyGO.Application.Services.Account
{
    public class UserAccountService : IUserAccountService
    {
        private readonly IUserRepository _userRepository;

        private readonly IMapper _mapper;

        private readonly ILogger<UserAccountService> _logger;

        private readonly IPasswordHasher _passwordHasher;

        private readonly IJwtTokenProvider _jwtTokenProvider;

        public UserAccountService(
            IUserRepository userRepository,
            IMapper mapper,
            ILogger<UserAccountService> logger,
            IPasswordHasher passwordHasher,
            IJwtTokenProvider jwtTokenProvider
        )
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _logger = logger;
            _passwordHasher = passwordHasher;
            _jwtTokenProvider = jwtTokenProvider;
        }

        public async Task<Result<Guid>> TryDeleteAccount(Guid id)
        {
            return await _userRepository.Delete(id);
        }

        public async Task<Result<UserDto?>> TryGetAccountById(Guid id)
        {
            Result<User?> result = await _userRepository.GetById(id);

            return result.MapTo(x => _mapper.Map<UserDto?>(x));
        }

        public async Task<Result<List<UserDto>>> TryGetAllAccount()
        {
            Result<List<User>> result = await _userRepository.GetAll();

            return result.MapTo(x => _mapper.Map<List<UserDto>>(x));
        }

        public async Task<Result<UserLoginResponseDto>> TryLogIn(UserLoginRequest userLogin)
        {
            Result<UserLoginResponse> result = await _userRepository.GetCredentialByEmail(
                userLogin.Email
            );

            if (!result.IsSuccess)
                return Result<UserLoginResponseDto>.Failure(result.ErrorMessage!);

            var dbSearchCred = result.Value!;

            bool IsAccess = IsSuccessUserLogin(userLogin, dbSearchCred);

            if (IsAccess)
            {
                var responseDto = new UserLoginResponseDto()
                {
                    Token = _jwtTokenProvider.GenerateToken(dbSearchCred),
                    Id = dbSearchCred.id,
                };

                return Result<UserLoginResponseDto>.Success(responseDto);
            }

            return Result<UserLoginResponseDto>.Failure("Invalid credentials");
        }

        public async Task<Result<Guid>> TryUpdateAccount(UserUpdateDto user)
        {
            return await TryUpdate(user);
        }

        public async Task<Result<Guid>> TryUpdateAccount(UserUpdateСredentialsDto user)
        {
            return await TryUpdate(user);
        }

        private async Task<Result<Guid>> TryUpdate<T>(T user)
        {
            User userModel = _mapper.Map<User>(user);

            return await _userRepository.Update(userModel);
        }

        private bool IsSuccessUserLogin(UserLoginRequest expected, UserLoginResponse actual)
        {
            string passwordHash = actual.PasswordHash;

            return expected.Email == actual.Email
                && expected.Password.VerifyPassword(passwordHash, _passwordHasher);
        }
    }
}
