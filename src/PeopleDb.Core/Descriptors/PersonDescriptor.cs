using System;
using PeopleDb.Core.Entities;
using PeopleDb.Core.Validation;

namespace PeopleDb.Core.Descriptors;

/// <summary>
/// Шаблонний метод: спільні атрибути (прізвище, ім'я) обробляються тут, а нащадок описує лише специфічні.
/// </summary>
public abstract class PersonDescriptor<T> : IPersonDescriptor where T : Person
{
    private static readonly FieldSpec[] CommonFields =
    {
        new("lastname", "Прізвище", Rules.Name, "літери, апостроф або дефіс, до 30 символів"),
        new("firstname", "Ім'я", Rules.Name, "літери, апостроф або дефіс, до 30 символів")
    };

    private FieldSpec[]? _fields;

    public string TypeName => typeof(T).Name;

    public abstract string Title { get; }

    public FieldSpec[] Fields => _fields ??= Combine(CommonFields, SpecificFields);

    protected abstract FieldSpec[] SpecificFields { get; }

    public bool Handles(Person person) => person.GetType() == typeof(T);

    public AttributeSet Describe(Person person)
    {
        var typed = (T)person;
        var attributes = new AttributeSet()
            .Add("firstname", typed.FirstName)
            .Add("lastname", typed.LastName);
        AddSpecific(typed, attributes);
        return attributes;
    }

    public Person Create(AttributeSet attributes) =>
        Build(attributes.Require("firstname"), attributes.Require("lastname"), attributes);

    protected abstract void AddSpecific(T person, AttributeSet attributes);

    protected abstract T Build(string firstName, string lastName, AttributeSet attributes);

    private static FieldSpec[] Combine(FieldSpec[] first, FieldSpec[] second)
    {
        var result = new FieldSpec[first.Length + second.Length];
        first.CopyTo(result, 0);
        second.CopyTo(result, first.Length);
        return result;
    }
}
