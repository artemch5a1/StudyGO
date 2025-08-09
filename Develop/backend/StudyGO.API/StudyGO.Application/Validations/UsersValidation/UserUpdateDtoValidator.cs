using FluentValidation;
using StudyGO.Contracts.Dtos.Users;

namespace StudyGO.Application.Validations.UsersValidation
{
    public class UserUpdateDtoValidator : AbstractValidator<UserUpdateDto>
    {
        public UserUpdateDtoValidator()
        {
            RuleFor(x => x.UserId).NotEmpty().WithMessage("Идентификатор пользователя обязателен");

            RuleFor(x => x.Surname)
                .NotEmpty()
                .WithMessage("Фамилия обязательна для заполнения")
                .MaximumLength(100)
                .WithMessage("Фамилия не должна превышать 100 символов")
                .Matches(@"^[a-zA-Zа-яА-ЯёЁ\-]+$")
                .WithMessage("Фамилия содержит недопустимые символы");

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Имя обязательно для заполнения")
                .MaximumLength(100)
                .WithMessage("Имя не должно превышать 100 символов")
                .Matches(@"^[a-zA-Zа-яА-ЯёЁ\-]+$")
                .WithMessage("Имя содержит недопустимые символы");

            RuleFor(x => x.Patronymic)
                .MaximumLength(100)
                .WithMessage("Отчество не должно превышать 100 символов")
                .Matches(@"^[a-zA-Zа-яА-ЯёЁ\-]*$")
                .WithMessage("Отчество содержит недопустимые символы")
                .When(x => !string.IsNullOrEmpty(x.Patronymic));

            RuleFor(x => x.Number)
                .MaximumLength(11)
                .WithMessage("Номер телефона не должен превышать 11 символов")
                .Matches(@"^[\d\+\-\(\)\s]*$")
                .WithMessage("Номер телефона содержит недопустимые символы")
                .When(x => !string.IsNullOrEmpty(x.Number));
        }
    }
}
