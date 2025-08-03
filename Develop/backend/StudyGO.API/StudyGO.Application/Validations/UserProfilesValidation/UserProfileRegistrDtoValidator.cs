using FluentValidation;
using StudyGO.Application.Validations.UsersValidation;
using StudyGO.Contracts.Dtos.UserProfiles;

namespace StudyGO.Application.Validations.UserProfilesValidation
{
    public class UserProfileRegistrDtoValidator : AbstractValidator<UserProfileRegistrDto>
    {
        public UserProfileRegistrDtoValidator()
        {
            RuleFor(x => x.User)
                .NotNull()
                .WithMessage("Данные пользователя обязательны")
                .SetValidator(new UserCreateDtoValidator());

            RuleFor(x => x.DateBirth)
                .NotEmpty()
                .WithMessage("Дата рождения обязательна")
                .LessThan(DateTime.Now.AddYears(-14))
                .WithMessage("Возраст должен быть не менее 14 лет")
                .GreaterThan(DateTime.Now.AddYears(-120))
                .WithMessage("Некорректная дата рождения");

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
