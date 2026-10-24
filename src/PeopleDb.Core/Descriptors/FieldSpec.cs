namespace PeopleDb.Core.Descriptors;

/// <summary>Опис поля для введення: ключ атрибуту, запрошення, регулярний вираз перевірки та підказка.</summary>
public sealed record FieldSpec(string Key, string Prompt, string Pattern, string Hint);
