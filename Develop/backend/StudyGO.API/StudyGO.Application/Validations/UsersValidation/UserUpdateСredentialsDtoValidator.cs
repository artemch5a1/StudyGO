using FluentValidation;
using StudyGO.Contracts.Dtos.Users;

namespace StudyGO.Application.Validations.UsersValidation
{
    public class UserUpdateСredentialsDtoValidator : BaseUserValidator<UserUpdateСredentialsDto>
    {
        public UserUpdateСredentialsDtoValidator()
        {
            RuleFor(x => x.UserId).NotEmpty().WithMessage("ID не должен быть пустым");

            AddEmailRule(x => x.Email);

            AddPasswordRule(x => x.Password);
        }
    }
}
