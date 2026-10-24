using System;
using System.Text.RegularExpressions;

namespace PeopleDb.Core.Validation;

/// <summary>
/// Єдине місце з регулярними виразами та перевіркою значень. Ті самі шаблони використовують
/// і конструктори сутностей, і консольне меню (підказки при введенні) — дублювання немає.
/// </summary>
public static class Rules
{
    /// <summary>Ім'я/прізвище: літери (укр./лат.), можливі апостроф чи дефіс усередині, до 30 символів.</summary>
    public const string Name = @"^(?=.{1,30}$)\p{L}+(?:['’\-]\p{L}+)*$";

    /// <summary>Студентський квиток: 2 великі літери + 6 цифр (наприклад, КВ123456 або KB123456).</summary>
    public const string StudentId = @"^\p{Lu}{2}\d{6}$";

    /// <summary>Курс: цифра від 1 до 6.</summary>
    public const string Course = @"^[1-6]$";

    /// <summary>Стать: Ч (чоловіча) або Ж (жіноча).</summary>
    public const string Gender = @"^[ЖЧжч]$";

    /// <summary>Гуртожиток у форматі «номер гуртожитку.кімната», наприклад 5.312.</summary>
    public const string Dormitory = @"^\d{1,2}\.\d{1,4}$";

    /// <summary>Звичайна адреса: містить літери, цифри, пробіли та знаки . , ' - /</summary>
    public const string Address = @"^(?=.*\p{L})[\p{L}\d\s.,'’\-/]{3,100}$";

    /// <summary>Місце проживання: або гуртожиток, або адреса.</summary>
    public const string Residence = Dormitory + "|" + Address;

    /// <summary>Музичний інструмент: літери, пробіли, апостроф, дефіс.</summary>
    public const string Instrument = @"^(?=.{2,30}$)\p{L}+(?:[ '’\-]\p{L}+)*$";

    /// <summary>Посвідчення музиканта: MU + 6 цифр.</summary>
    public const string MusicianId = @"^MU\d{6}$";

    /// <summary>Номер ліцензії пілота: PL- + 6 цифр.</summary>
    public const string LicenseNumber = @"^PL-\d{6}$";

    /// <summary>Лічильник/кількість: до 6 цифр.</summary>
    public const string Counter = @"^\d{1,6}$";

    /// <summary>Перевіряє значення за шаблоном; повертає його без крайніх пробілів або кидає виняток.</summary>
    public static string Require(string? value, string pattern, string field)
    {
        string trimmed = value?.Trim() ?? string.Empty;
        if (!Regex.IsMatch(trimmed, pattern))
        {
            throw new ArgumentException($"Некоректне значення поля «{field}»: '{value}'.", field);
        }

        return trimmed;
    }

    /// <summary>Лічильники (години, заняття) не можуть бути від'ємними.</summary>
    public static int RequireNonNegative(int value, string field)
    {
        if (value < 0)
        {
            throw new ArgumentOutOfRangeException(field, value, $"Поле «{field}» не може бути від'ємним.");
        }

        return value;
    }
}
