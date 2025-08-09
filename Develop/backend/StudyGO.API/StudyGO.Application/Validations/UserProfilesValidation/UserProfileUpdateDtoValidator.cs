using FluentValidation;
using StudyGO.Contracts.Dtos.UserProfiles;

namespace StudyGO.Application.Validations.UserProfilesValidation
{
    public class UserProfileUpdateDtoValidator : AbstractValidator<UserProfileUpdateDto>
    {
        public UserProfileUpdateDtoValidator()
        {
            RuleFor(x => x.UserId).NotEmpty().WithMessage("ID пользователя обязателен");

            RuleFor(x => x.DateBirth)
                .NotEmpty()
                .WithMessage("Дата рождения обязательна");

            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage("Описание обязательно")
                .MaximumLength(1000)
                .WithMessage("Описание не должно превышать 1000 символов")
                .MinimumLength(20)
                .WithMessage("Описание должно содержать минимум 20 символов");
        }
    }
}
