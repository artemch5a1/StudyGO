using AutoMapper;
using Microsoft.Extensions.Logging;
using StudyGO.Application.Abstractions;
using StudyGO.Application.Extensions;
using StudyGO.Contracts.Dtos.UserProfiles;
using StudyGO.Core.Abstractions.Repositories;
using StudyGO.Core.Abstractions.Services.Account;

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

        public Task<List<UserProfileDto>> GetAllUserProfiles(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<UserProfileDto?> TryGetUserProfileById(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<Guid> TryRegistr(UserProfileRegistrDto profile)
        {
            profile.User.Password = profile.User.Password.HashedPassword(_passwordHasher);
        }

        public Task<bool> TryUpdateUserProfile(UserProfileUpdateDto newProfile)
        {
            throw new NotImplementedException();
        }
    }
}
