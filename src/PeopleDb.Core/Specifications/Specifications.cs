using System;
using PeopleDb.Core.Entities;

namespace PeopleDb.Core.Specifications;

/// <summary>Умова відбору осіб. Нові запити додаються новими класами, без змін репозиторію.</summary>
public interface IPersonSpecification
{
    bool IsSatisfiedBy(Person person);
}

/// <summary>Завдання варіанту №3: студентки 1-го курсу, які проживають у гуртожитку.</summary>
public sealed class FirstYearFemaleInDormitorySpecification : IPersonSpecification
{
    public bool IsSatisfiedBy(Person person) =>
        person is Student { Course: 1, Gender: Gender.Female, Residence.IsDormitory: true };
}

/// <summary>Пошук за прізвищем (без урахування регістру).</summary>
public sealed class LastNameSpecification : IPersonSpecification
{
    private readonly string _lastName;

    public LastNameSpecification(string lastName) => _lastName = lastName;

    public bool IsSatisfiedBy(Person person) =>
        string.Equals(person.LastName, _lastName, StringComparison.OrdinalIgnoreCase);
}

/// <summary>Пошук за унікальним ідентифікатором (без урахування регістру).</summary>
public sealed class IdSpecification : IPersonSpecification
{
    private readonly string _id;

    public IdSpecification(string id) => _id = id;

    public bool IsSatisfiedBy(Person person) => string.Equals(person.Id, _id, StringComparison.OrdinalIgnoreCase);
}

public static class PersonSpecificationExtensions
{
    /// <summary>Відбирає з масиву особи, що задовольняють умову (масив замість колекцій).</summary>
    public static Person[] Filter(this Person[] source, IPersonSpecification specification)
    {
        Person[] result = new Person[source.Length];
        int count = 0;
        foreach (Person person in source)
        {
            if (specification.IsSatisfiedBy(person))
            {
                result[count++] = person;
            }
        }

        Array.Resize(ref result, count);
        return result;
    }
}
