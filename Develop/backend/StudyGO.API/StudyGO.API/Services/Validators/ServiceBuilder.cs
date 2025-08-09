using FluentValidation;
using StudyGO.Application.Validations.Service;
using StudyGO.Application.Validations.TutorProfileValidation;
using StudyGO.Application.Validations.UserProfilesValidation;
using StudyGO.Application.Validations.UsersValidation;
using StudyGO.Contracts.Dtos.TutorProfiles;
using StudyGO.Contracts.Dtos.UserProfiles;
using StudyGO.Contracts.Dtos.Users;
using StudyGO.Core.Abstractions.ValidationService;

namespace StudyGO.API.Services;

public partial class ServiceBuilder
{
    private void CofigureValidators()
    {
        _services.AddScoped<IValidationService, ValidationService>();

        _services.AddScoped<IValidator<UserCreateDto>, UserCreateDtoValidator>();

        _services.AddScoped<IValidator<UserProfileRegistrDto>, UserProfileRegistryDtoValidator>();

        _services.AddScoped<IValidator<UserProfileUpdateDto>, UserProfileUpdateDtoValidator>();

        _services.AddScoped<IValidator<UserUpdateDto>, UserUpdateDtoValidator>();

        _services.AddScoped<
            IValidator<UserUpdateСredentialsDto>,
            UserUpdateСredentialsDtoValidator
        >();

        _services.AddScoped<IValidator<TutorProfileRegistrDto>, TutorProfileRegistryDtoValidation>();

        _services.AddScoped<IValidator<TutorProfileUpdateDto>, TutorProfileUpdateDtoValidator>();
    }
}
