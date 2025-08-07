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
using StudyGO.Core.Abstractions.ValidationService;
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

        private readonly IValidationService _validationService;

        public UserAccountService(
            IUserRepository userRepository,
            IMapper mapper,
            ILogger<UserAccountService> logger,
            IPasswordHasher passwordHasher,
            IJwtTokenProvider jwtTokenProvider,
            IValidationService validationService
        )
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _logger = logger;
            _passwordHasher = passwordHasher;
            _jwtTokenProvider = jwtTokenProvider;
            _validationService = validationService;
        }

        public async Task<Result<Guid>> TryDeleteAccount(
            Guid id,
            CancellationToken cancellationToken = default
        )
        {
            return await _userRepository.Delete(id, cancellationToken);
        }

        public async Task<Result<UserDto?>> TryGetAccountById(
            Guid id,
            CancellationToken cancellationToken = default
        )
        {
            Result<User?> result = await _userRepository.GetById(id, cancellationToken);

            return result.MapDataTo(x => _mapper.Map<UserDto?>(x));
        }

        public async Task<Result<List<UserDto>>> TryGetAllAccount(
            CancellationToken cancellationToken = default
        )
        {
            Result<List<User>> result = await _userRepository.GetAll(cancellationToken);

            return result.MapDataTo(_mapper.Map<List<UserDto>>);
        }

        public async Task<Result<UserLoginResponseDto>> TryLogIn(
            UserLoginRequest userLogin,
            CancellationToken cancellationToken = default
        )
        {
            Result<UserLoginResponse> result = await _userRepository.GetCredentialByEmail(
                userLogin.Email,
                cancellationToken
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

        public async Task<Result<Guid>> TryUpdateAccount(
            UserUpdateDto user,
            CancellationToken cancellationToken = default
        )
        {
            var validationResult = await _validationService.ValidateAsync(user, cancellationToken);

            if (!validationResult.IsSuccess)
            {
                return Result<Guid>.Failure(
                    validationResult.ErrorMessage ?? string.Empty,
                    validationResult.ErrorType
                );
            }

            User userModel = _mapper.Map<User>(user);

            return await _userRepository.Update(userModel, cancellationToken);
        }

        public async Task<Result<Guid>> TryUpdateAccount(
            UserUpdateСredentialsDto user,
            CancellationToken cancellationToken = default
        )
        {
            var validationResult = await _validationService.ValidateAsync(user, cancellationToken);

            if (!validationResult.IsSuccess)
            {
                return Result<Guid>.Failure(
                    validationResult.ErrorMessage ?? string.Empty,
                    validationResult.ErrorType
                );
            }

            var result = await _userRepository.GetById(user.UserId, cancellationToken);

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

            return await _userRepository.UpdateСredentials(userModel, cancellationToken);
        }

        private bool IsSuccessUserLogin(UserLoginRequest expected, UserLoginResponse actual)
        {
            string passwordHash = actual.PasswordHash;

            return expected.Email == actual.Email
                && expected.Password.VerifyPassword(passwordHash, _passwordHasher);
        }
    }
}
