using FluentValidation;
using StudyGO.Application.Validations.Base;
using StudyGO.Contracts.Dtos.Users;

namespace StudyGO.Application.Validations.UsersValidation
{
    public class UserUpdateDtoValidator : BaseUserValidator<UserUpdateDto>
    {
        public UserUpdateDtoValidator()
        {
            RuleFor(x => x.UserId).NotEmpty().WithMessage("Идентификатор пользователя обязателен");

            AddSurnameRule(x => x.Surname);

            AddNameRule(x => x.Name);

            AddPatronymicRule(x => x.Patronymic);

            AddNumberRule(x => x.Number);
        }
    }
}
