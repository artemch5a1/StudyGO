using AutoMapper;
using Microsoft.Extensions.Logging;
using StudyGO.Application.Extensions;
using StudyGO.Contracts.Dtos.TutorProfiles;
using StudyGO.Contracts.Result;
using StudyGO.Core.Abstractions.Repositories;
using StudyGO.Core.Abstractions.Services.Account;
using StudyGO.Core.Abstractions.Utils;
using StudyGO.Core.Enums;
using StudyGO.Core.Extensions;
using StudyGO.Core.Models;

namespace StudyGO.Application.Services.Account
{
    public class TutorProfileService : ITutorProfileService
    {
        private readonly ITutorProfileRepository _userRepository;

        private readonly IMapper _mapper;

        private readonly ILogger<TutorProfileService> _logger;

        private readonly IPasswordHasher _passwordHasher;

        public TutorProfileService(
            ITutorProfileRepository userRepository,
            IMapper mapper,
            ILogger<TutorProfileService> logger,
            IPasswordHasher passwordHasher
        )
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _logger = logger;
            _passwordHasher = passwordHasher;
        }

        public async Task<Result<List<TutorProfileDto>>> GetAllUserProfiles()
        {
            var result = await _userRepository.GetAll();

            return result.MapTo(_mapper.Map<List<TutorProfileDto>>);
        }

        public async Task<Result<TutorProfileDto?>> TryGetUserProfileById(Guid userId)
        {
            var result = await _userRepository.GetById(userId);

            return result.MapTo(_mapper.Map<TutorProfileDto?>);
        }

        public async Task<Result<Guid>> TryRegistr(TutorProfileRegistrDto profile)
        {
            profile.User.Password = profile.User.Password.HashedPassword(_passwordHasher);

            TutorProfile profileModel = _mapper.Map<TutorProfile>(profile);

            profileModel.User!.Role = RolesEnum.tutor.GetString();

            return await _userRepository.Create(profileModel);
        }

        public Task<Result<Guid>> TryUpdateUserProfile(TutorProfileUpdateDto newProfile)
        {
            throw new NotImplementedException();
        }
    }
}
