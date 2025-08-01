using AutoMapper;
using Microsoft.Extensions.Logging;
using StudyGO.Application.Abstractions;
using StudyGO.Application.Extensions;
using StudyGO.Contracts.Dtos.UserProfiles;
using StudyGO.Core.Abstractions.Repositories;
using StudyGO.Core.Abstractions.Services.Account;
using StudyGO.Core.Models;

namespace StudyGO.Application.Services.Account
{
    public class UserProfileService : IUserProfileService
    {
        private IUserProfileRepository _userRepository;

        private IMapper _mapper;

        private ILogger _logger;

        private IPasswordHasher _passwordHasher;

        public UserProfileService(
            IUserProfileRepository userRepository,
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

        public async Task<List<UserProfileDto>> GetAllUserProfiles()
        {
            List<UserProfile> users = await _userRepository.GetAll();

            return _mapper.Map<List<UserProfileDto>>(users);
        }

        public async Task<UserProfileDto?> TryGetUserProfileById(Guid userId)
        {
            UserProfile? users = await _userRepository.GetById(userId);

            return _mapper.Map<UserProfileDto?>(users);
        }

        public async Task<Guid> TryRegistr(UserProfileRegistrDto profile)
        {
            profile.User.Password = profile.User.Password.HashedPassword(_passwordHasher);

            UserProfile user = _mapper.Map<UserProfile>(profile);

            return await _userRepository.Create(user);
        }

        public async Task<bool> TryUpdateUserProfile(UserProfileUpdateDto newProfile)
        {
            UserProfile user = _mapper.Map<UserProfile>(newProfile);

            return await _userRepository.Update(user);
        }
    }
}
