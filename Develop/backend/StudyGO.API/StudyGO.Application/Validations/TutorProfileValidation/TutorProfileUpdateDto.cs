using FluentValidation;
using StudyGO.Contracts.Dtos.TutorProfiles;

namespace StudyGO.Application.Validations.TutorProfileValidation
{
    public class TutorProfileUpdateDtoValidator : AbstractValidator<TutorProfileUpdateDto>
    {
        public TutorProfileUpdateDtoValidator()
        {
            RuleFor(x => x.UserId).NotEmpty().WithMessage("ID пользователя обязателен");

            RuleFor(x => x.Bio)
                .NotEmpty()
                .WithMessage("Биография обязательна")
                .MaximumLength(1000)
                .WithMessage("Биография не должна превышать 1000 символов")
                .MinimumLength(20)
                .WithMessage("Биография должна содержать минимум 20 символов");

            RuleFor(x => x.City).NotEmpty().WithMessage("Необходимо указать город");

            RuleFor(x => x.PricePerHour)
                .NotEmpty()
                .WithMessage("Необходимо указать цену за час обучения");

            RuleFor(x => x.FormatId).NotEmpty().WithMessage("Необходимо указать формат обучения");
            
            RuleFor(x => x.SubjectsId).NotEmpty().WithMessage("Нужно указать хотя бы один предмет");
        }
    }
}
