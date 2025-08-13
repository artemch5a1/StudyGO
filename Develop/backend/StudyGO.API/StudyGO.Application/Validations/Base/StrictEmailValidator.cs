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
            .WithMessage("������������ ������ email. ����������� ���������� ������: user@example.com");
    }

    private static bool IsValidStrictEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email) || email.Length > 255)
            return false;

        try
        {
            // �������� ������� ��� ��������
            const string pattern = """
                                   ^
                                                   (?!\.)                            # �� ����� ���������� � �����
                                                   (?!.*\.\.)                        # �� ����� ��������� ��� ����� ������
                                                   [a-zA-Z0-9._%+-]+                 # ��������� �����
                                                   @                                 
                                                   (?!-)(?!.*--)                     # ����� �� ����� ����������/������������� �� -
                                                   ([a-zA-Z0-9-]+\.)+                # ���������
                                                   [a-zA-Z]{2,}                      # ��������������� �����
                                                   (?!\.)                            # �� ����� ������������� ������
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