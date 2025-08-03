using FluentValidation;
using StudyGO.Contracts.Dtos.Users;

namespace StudyGO.Application.Validations.UsersValidation
{
    public class UserUpdateСredentialsDtoValidator : AbstractValidator<UserUpdateСredentialsDto>
    {
        public UserUpdateСredentialsDtoValidator()
        {
            RuleFor(x => x.UserId).NotEmpty().WithMessage("ID не должен быть пустым");

            RuleFor(x => x.Email).EmailAddress().WithMessage("Некорретный email");

            RuleFor(x => x.Password)
                .MinimumLength(10)
                .WithMessage("Пароль не может быть меньше 10 символов")
                .MaximumLength(150)
                .WithMessage("Пароль не может превышать 150 символов");
        }
    }
}
