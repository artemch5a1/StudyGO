using StudyGO.Application.Validations.Base;
using StudyGO.Contracts.Dtos.Users;

namespace StudyGO.Application.Validations.UsersValidation
{
    public class UserCreateDtoValidator : BaseUserValidator<UserCreateDto>
    {
        public UserCreateDtoValidator()
        {
            AddEmailRule(x => x.Email);

            AddPasswordRule(x => x.Password);

            AddSurnameRule(x => x.Surname);

            AddNameRule(x => x.Name);

            AddPatronymicRule(x => x.Patronymic);

            AddNumberRule(x => x.Number);
        }
    }
}
