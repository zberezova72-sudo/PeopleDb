using PeopleDb.Core.Validation;

namespace PeopleDb.Core.Entities;

/// <summary>Базовий клас: обов'язкові FirstName та LastName перевіряються регулярним виразом.</summary>
public abstract class Person : IPerson
{
    protected Person(string firstName, string lastName)
    {
        FirstName = Rules.Require(firstName, Rules.Name, "Ім'я");
        LastName = Rules.Require(lastName, Rules.Name, "Прізвище");
    }

    public string FirstName { get; }

    public string LastName { get; }

    public abstract string Id { get; }
}
