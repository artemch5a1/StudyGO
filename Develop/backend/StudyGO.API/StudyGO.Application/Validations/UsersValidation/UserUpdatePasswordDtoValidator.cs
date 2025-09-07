using FluentValidation;
using StudyGO.Application.Validations.Base;
using StudyGO.Contracts.Dtos.Users;

namespace StudyGO.Application.Validations.UsersValidation
{
    public class UserUpdatePasswordDtoValidator : BaseUserValidator<UserUpdatePasswordDto>
    {
        public UserUpdatePasswordDtoValidator()
        {
            RuleFor(x => x.UserId).NotEmpty().WithMessage("ID не должен быть пустым");

            AddPasswordRule(x => x.Password);
        }
    }
}
