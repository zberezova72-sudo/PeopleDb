namespace PeopleDb.Core.Entities;

/// <summary>Спільний контракт усіх осіб у базі: ім'я, прізвище та унікальний ідентифікатор.</summary>
public interface IPerson
{
    string FirstName { get; }
    string LastName { get; }

    /// <summary>Унікальний ідентифікатор (студентський квиток, ліцензія тощо).</summary>
    string Id { get; }
}
