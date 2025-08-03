using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.Logging;
using StudyGO.Application.Extensions;
using StudyGO.Contracts.Dtos.TutorProfiles;
using StudyGO.Contracts.Dtos.UserProfiles;
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

        private readonly IValidator<TutorProfileRegistrDto> _registrValidator;

        private readonly IValidator<TutorProfileUpdateDto> _updateValidor;

        public TutorProfileService(
            ITutorProfileRepository userRepository,
            IMapper mapper,
            ILogger<TutorProfileService> logger,
            IPasswordHasher passwordHasher,
            IValidator<TutorProfileRegistrDto> registrValidator,
            IValidator<TutorProfileUpdateDto> updateValidor
        )
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _logger = logger;
            _passwordHasher = passwordHasher;
            _registrValidator = registrValidator;
            _updateValidor = updateValidor;
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
            var validatorResult = _registrValidator.Validate(profile);

            if (!validatorResult.IsValid)
                return Result<Guid>.Failure(
                    validatorResult.Errors.FirstOrDefault()?.ErrorMessage ?? string.Empty
                );

            profile.User.Password = profile.User.Password.HashedPassword(_passwordHasher);

            TutorProfile profileModel = _mapper.Map<TutorProfile>(profile);

            profileModel.User!.Role = RolesEnum.tutor.GetString();

            return await _userRepository.Create(profileModel);
        }

        public async Task<Result<Guid>> TryUpdateUserProfile(TutorProfileUpdateDto newProfile)
        {
            var validatorResult = _updateValidor.Validate(newProfile);

            if (!validatorResult.IsValid)
                return Result<Guid>.Failure(
                    validatorResult.Errors.FirstOrDefault()?.ErrorMessage ?? string.Empty
                );

            return await _userRepository.Update(_mapper.Map<TutorProfile>(newProfile));
        }
    }
}
