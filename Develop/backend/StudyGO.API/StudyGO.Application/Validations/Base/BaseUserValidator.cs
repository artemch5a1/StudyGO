using System.Linq.Expressions;
using FluentValidation;

namespace StudyGO.Application.Validations.Base;

public abstract class BaseUserValidator<T> : AbstractValidator<T>
    where T : class
{
    protected void AddEmailRule(Expression<Func<T, string>> expression)
    {
        RuleFor(expression)
            .NotEmpty()
            .WithMessage("Email обязателен")
            .StrictEmail()
            .MaximumLength(255)
            .WithMessage("Слишком длинный email");
    }

    protected void AddNameRule(Expression<Func<T, string>> expression)
    {
        RuleFor(expression)
            .NotEmpty()
            .WithMessage("Имя обязательно для заполнения")
            .MaximumLength(100)
            .WithMessage("Имя не должно превышать 100 символов")
            .Matches(@"^[a-zA-Zа-яА-ЯёЁ\-]+$")
            .WithMessage("Имя содержит недопустимые символы");
    }

    protected void AddSurnameRule(Expression<Func<T, string>> expression)
    {
        RuleFor(expression)
            .NotEmpty()
            .WithMessage("Фамилия обязательна для заполнения")
            .MaximumLength(100)
            .WithMessage("Фамилия не должна превышать 100 символов")
            .Matches(@"^[a-zA-Zа-яА-ЯёЁ\-]+$")
            .WithMessage("Фамилия содержит недопустимые символы");
    }

    protected void AddPatronymicRule(Expression<Func<T, string>> expression)
    {
        RuleFor(expression)
            .MaximumLength(100)
            .WithMessage("Отчество не должно превышать 100 символов")
            .Matches(@"^[a-zA-Zа-яА-ЯёЁ\-]*$")
            .WithMessage("Отчество содержит недопустимые символы")
            .When(x => !string.IsNullOrEmpty(expression.Compile()(x)));
    }

    protected void AddPasswordRule(Expression<Func<T, string>> expression)
    {
        RuleFor(expression)
            .NotEmpty()
            .WithMessage("Пароль обязателен")
            .MinimumLength(8)
            .WithMessage("Пароль должен содержать минимум 8 символов");
    }

    protected void AddNumberRule(Expression<Func<T, string>> expression)
    {
        RuleFor(expression)
            .MaximumLength(11).WithMessage("Номер телефона не должен превышать 11 символов")
            .Matches(@"^[\d\+\-\(\)\s]*$").WithMessage("Номер содержит недопустимые символы")
            .When(x => !string.IsNullOrEmpty(expression.Compile()(x)));
    }
}