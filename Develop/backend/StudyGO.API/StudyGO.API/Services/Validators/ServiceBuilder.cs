using FluentValidation;
using StudyGO.Application.Validations.UserProfilesValidation;
using StudyGO.Application.Validations.UsersValidation;
using StudyGO.Contracts.Dtos.UserProfiles;
using StudyGO.Contracts.Dtos.Users;

namespace StudyGO.API.Services;

public partial class ServiceBuilder
{
    private void CofigureValidators()
    {
        _services.AddScoped<IValidator<UserCreateDto>, UserCreateDtoValidator>();

        _services.AddScoped<IValidator<UserProfileRegistrDto>, UserProfileRegistrDtoValidator>();

        _services.AddScoped<IValidator<UserProfileUpdateDto>, UserProfileUpdateDtoValidator>();

        _services.AddScoped<IValidator<UserUpdateDto>, UserUpdateDtoValidator>();

        _services.AddScoped<
            IValidator<UserUpdateСredentialsDto>,
            UserUpdateСredentialsDtoValidator
        >();
    }
}
