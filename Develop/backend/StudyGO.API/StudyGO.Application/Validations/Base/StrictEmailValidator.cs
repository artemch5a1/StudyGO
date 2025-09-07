using System.Text.RegularExpressions;
using FluentValidation;

namespace StudyGO.Application.Validations.Base;

public static class StrictEmailValidator
{
    public static IRuleBuilderOptions<T, string> StrictEmail<T>(
        this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .Must(IsValidStrictEmail)
            .WithMessage("Некорректный формат email. Используйте правильный формат: user@example.com");
    }

    private static bool IsValidStrictEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email) || email.Length > 255)
            return false;

        try
        {
            // Основной паттерн для проверки
            const string pattern = """
                                   ^
                                                   (?!\.)                            # Не может начинаться с точки
                                                   (?!.*\.\.)                        # Не может содержать две точки подряд
                                                   [a-zA-Z0-9._%+-]+                 # Локальная часть
                                                   @                                 
                                                   (?!-)(?!.*--)                     # Домен не может начинаться/заканчиваться на -
                                                   ([a-zA-Z0-9-]+\.)+                # Поддомены
                                                   [a-zA-Z]{2,}                      # Верхнеуровневый домен
                                                   (?!\.)                            # Не может заканчиваться точкой
                                                   $
                                   """;

            return Regex.IsMatch(email.Trim(), pattern, 
                RegexOptions.IgnorePatternWhitespace, 
                TimeSpan.FromMilliseconds(200));
        }
        catch (RegexMatchTimeoutException)
        {
            return false;
        }
    }
}