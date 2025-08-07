using AutoMapper;
using Microsoft.Extensions.Logging;
using StudyGO.Application.Extensions;
using StudyGO.Contracts.Dtos.UserProfiles;
using StudyGO.Contracts.Result;
using StudyGO.Core.Abstractions.Repositories;
using StudyGO.Core.Abstractions.Services.Account;
using StudyGO.Core.Abstractions.Utils;
using StudyGO.Core.Abstractions.ValidationService;
using StudyGO.Core.Enums;
using StudyGO.Core.Extensions;
using StudyGO.Core.Models;

namespace StudyGO.Application.Services.Account
{
    public class UserProfileService : IUserProfileService
    {
        private readonly IUserProfileRepository _userRepository;

        private readonly IMapper _mapper;

        private readonly ILogger<UserProfileService> _logger;

        private readonly IPasswordHasher _passwordHasher;

        private readonly IValidationService _validationService;

        public UserProfileService(
            IUserProfileRepository userRepository,
            IMapper mapper,
            ILogger<UserProfileService> logger,
            IPasswordHasher passwordHasher,
            IValidationService validationService
        )
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _logger = logger;
            _passwordHasher = passwordHasher;
            _validationService = validationService;
        }

        public async Task<Result<List<UserProfileDto>>> GetAllUserProfiles(
            CancellationToken cancellationToken = default
        )
        {
            var result = await _userRepository.GetAll(cancellationToken);

            return result.MapDataTo(_mapper.Map<List<UserProfileDto>>);
        }

        public async Task<Result<UserProfileDto?>> TryGetUserProfileById(
            Guid userId,
            CancellationToken cancellationToken = default
        )
        {
            var result = await _userRepository.GetById(userId, cancellationToken);

            return result.MapDataTo(_mapper.Map<UserProfileDto?>);
        }

        public async Task<Result<Guid>> TryRegistr(
            UserProfileRegistrDto profile,
            CancellationToken cancellationToken = default
        )
        {
            var validatorResult = await _validationService.ValidateAsync(
                profile,
                cancellationToken
            );

            if (!validatorResult.IsSuccess)
                return Result<Guid>.Failure(
                    validatorResult.Value?.FirstOrDefault()?.ErrorMessage ?? string.Empty
                );

            profile.User.Password = profile.User.Password.HashedPassword(_passwordHasher);

            UserProfile profileModel = _mapper.Map<UserProfile>(profile);

            profileModel.User!.Role = RolesEnum.user.GetString();

            return await _userRepository.Create(profileModel, cancellationToken);
        }

        public async Task<Result<Guid>> TryUpdateUserProfile(
            UserProfileUpdateDto newProfile,
            CancellationToken cancellationToken = default
        )
        {
            var validatorResult = await _validationService.ValidateAsync(
                newProfile,
                cancellationToken
            );

            if (!validatorResult.IsSuccess)
                return Result<Guid>.Failure(
                    validatorResult.Value?.FirstOrDefault()?.ErrorMessage ?? string.Empty
                );

            UserProfile user = _mapper.Map<UserProfile>(newProfile);

            return await _userRepository.Update(user, cancellationToken);
        }
    }
}
