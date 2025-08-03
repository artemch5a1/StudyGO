using AutoMapper;
using FluentValidation;
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

        private readonly IValidator<UserUpdateDto> _validatorUpdate;

        private readonly IValidator<UserUpdateСredentialsDto> _validatorUpdateCredentials;

        public UserAccountService(
            IUserRepository userRepository,
            IMapper mapper,
            ILogger<UserAccountService> logger,
            IPasswordHasher passwordHasher,
            IJwtTokenProvider jwtTokenProvider,
            IValidator<UserUpdateDto> validator,
            IValidator<UserUpdateСredentialsDto> validatorUpdateCredentials
        )
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _logger = logger;
            _passwordHasher = passwordHasher;
            _jwtTokenProvider = jwtTokenProvider;
            _validatorUpdate = validator;
            _validatorUpdateCredentials = validatorUpdateCredentials;
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

            return result.MapTo(_mapper.Map<List<UserDto>>);
        }

        public async Task<Result<UserLoginResponseDto>> TryLogIn(UserLoginRequest userLogin)
        {
            Result<UserLoginResponse> result = await _userRepository.GetCredentialByEmail(
                userLogin.Email
            );

            if (!result.IsSuccess)
                return Result<UserLoginResponseDto>.Failure(result.ErrorMessage!);

            var dbSearchCred = result.Value ?? new();

            bool isAccess = IsSuccessUserLogin(userLogin, dbSearchCred);

            if (isAccess)
            {
                var responseDto = new UserLoginResponseDto()
                {
                    Token = _jwtTokenProvider.GenerateToken(dbSearchCred),
                    Id = dbSearchCred.Id,
                };

                return Result<UserLoginResponseDto>.Success(responseDto);
            }

            return Result<UserLoginResponseDto>.Failure("Invalid credentials");
        }

        public async Task<Result<Guid>> TryUpdateAccount(UserUpdateDto user)
        {
            var validationResult = await _validatorUpdate.ValidateAsync(user);

            if (!validationResult.IsValid)
                return Result<Guid>.Failure(
                    validationResult.Errors.FirstOrDefault()?.ErrorMessage ?? string.Empty
                );

            User userModel = _mapper.Map<User>(user);

            return await _userRepository.Update(userModel);
        }

        public async Task<Result<Guid>> TryUpdateAccount(UserUpdateСredentialsDto user)
        {
            var validationResult = await _validatorUpdateCredentials.ValidateAsync(user);

            if (!validationResult.IsValid)
                return Result<Guid>.Failure(
                    validationResult.Errors.FirstOrDefault()?.ErrorMessage ?? string.Empty
                );

            var result = await _userRepository.GetById(user.UserId);

            if (!result.IsSuccess)
                return Result<Guid>.Failure("Неверный ID");

            var check = user.OldPassword.VerifyPassword(
                result.Value!.PasswordHash,
                _passwordHasher
            );

            if (!check)
                return Result<Guid>.Failure("Неверный пароль");

            user.Password = user.Password.HashedPassword(_passwordHasher);

            User userModel = _mapper.Map<User>(user);

            return await _userRepository.UpdateСredentials(userModel);
        }

        private bool IsSuccessUserLogin(UserLoginRequest expected, UserLoginResponse actual)
        {
            string passwordHash = actual.PasswordHash;

            return expected.Email == actual.Email
                && expected.Password.VerifyPassword(passwordHash, _passwordHasher);
        }
    }
}
