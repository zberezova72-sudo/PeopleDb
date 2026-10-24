using System;
using System.Globalization;
using System.IO;

namespace PeopleDb.Core.Descriptors;

/// <summary>
/// Набір пар «атрибут — значення» одного запису. Реалізований на масивах (колекції заборонені).
/// Слугує проміжною ланкою між файлом (FileStorage) та сутністю: атрибути надходять поелементно.
/// </summary>
public sealed class AttributeSet
{
    private string[] _keys = Array.Empty<string>();
    private string[] _values = Array.Empty<string>();

    public int Count => _keys.Length;

    public string KeyAt(int index) => _keys[index];

    public string ValueAt(int index) => _values[index];

    public AttributeSet Add(string key, string value)
    {
        Array.Resize(ref _keys, _keys.Length + 1);
        Array.Resize(ref _values, _values.Length + 1);
        _keys[^1] = key;
        _values[^1] = value;
        return this;
    }

    public AttributeSet Add(string key, int value) => Add(key, value.ToString(CultureInfo.InvariantCulture));

    /// <summary>Значення обов'язкового атрибуту; якщо його немає — помилка формату даних.</summary>
    public string Require(string key) =>
        TryGet(key, out string value) ? value : throw new InvalidDataException($"Відсутній обов'язковий атрибут «{key}».");

    public int RequireInt(string key) => ParseInt(key, Require(key));

    /// <summary>Необов'язковий числовий атрибут: якщо його немає — значення за умовчанням.</summary>
    public int GetInt(string key, int fallback) => TryGet(key, out string value) ? ParseInt(key, value) : fallback;

    private bool TryGet(string key, out string value)
    {
        for (int i = 0; i < _keys.Length; i++)
        {
            if (string.Equals(_keys[i], key, StringComparison.OrdinalIgnoreCase))
            {
                value = _values[i];
                return true;
            }
        }

        value = string.Empty;
        return false;
    }

    private static int ParseInt(string key, string text) =>
        int.TryParse(text, NumberStyles.None, CultureInfo.InvariantCulture, out int number)
            ? number
            : throw new InvalidDataException($"Атрибут «{key}» має бути невід'ємним цілим числом, а не '{text}'.");
}
