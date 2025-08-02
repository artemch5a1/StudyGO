using AutoMapper;
using Microsoft.Extensions.Logging;
using StudyGO.Application.Extensions;
using StudyGO.Contracts.Dtos.UserProfiles;
using StudyGO.Contracts.Result;
using StudyGO.Core.Abstractions.Repositories;
using StudyGO.Core.Abstractions.Services.Account;
using StudyGO.Core.Abstractions.Utils;
using StudyGO.Core.Enums;
using StudyGO.Core.Extensions;
using StudyGO.Core.Models;
using System.Collections.Generic;

namespace StudyGO.Application.Services.Account
{
    public class UserProfileService : IUserProfileService
    {
        private readonly IUserProfileRepository _userRepository;

        private readonly IMapper _mapper;

        private readonly ILogger<UserProfileService> _logger;

        private readonly IPasswordHasher _passwordHasher;

        public UserProfileService(
            IUserProfileRepository userRepository,
            IMapper mapper,
            ILogger<UserProfileService> logger,
            IPasswordHasher passwordHasher
        )
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _logger = logger;
            _passwordHasher = passwordHasher;
        }

        public async Task<Result<List<UserProfileDto>>> GetAllUserProfiles()
        {
            var result = await _userRepository.GetAll();

            return result.MapTo(_mapper.Map<List<UserProfileDto>>);
        }

        public async Task<Result<UserProfileDto?>> TryGetUserProfileById(Guid userId)
        {
            var result = await _userRepository.GetById(userId);

            return result.MapTo(_mapper.Map<UserProfileDto?>);
        }

        public async Task<Result<Guid>> TryRegistr(UserProfileRegistrDto profile)
        {
            profile.User.Password = profile.User.Password.HashedPassword(_passwordHasher);

            UserProfile user = _mapper.Map<UserProfile>(profile);

            user.User!.Role = RolesEnum.user.GetString();

            return await _userRepository.Create(user);
        }

        public async Task<Result<Guid>> TryUpdateUserProfile(UserProfileUpdateDto newProfile)
        {
            UserProfile user = _mapper.Map<UserProfile>(newProfile);

            return await _userRepository.Update(user);
        }
    }
}
