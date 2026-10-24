using System;
using FileStorage;
using PeopleDb.Core.Descriptors;
using PeopleDb.Core.Entities;
using PeopleDb.Core.Specifications;

namespace PeopleDb.Core.Persistence;

/// <summary>
/// Зберігає осіб в одному файлі-джерелі різних типів. Типи та атрибути визначають дескриптори,
/// файловий ввід-вивід виконує <see cref="IRecordStore"/> (окремий проект) — поелементно.
/// </summary>
public sealed class PersonRepository : IPersonRepository
{
    private readonly IRecordStore _store;
    private readonly DescriptorRegistry _descriptors;

    public PersonRepository(IRecordStore store, DescriptorRegistry descriptors)
    {
        _store = store;
        _descriptors = descriptors;
    }

    public Person[] GetAll()
    {
        Person[] people = Array.Empty<Person>();
        using IRecordReader reader = _store.OpenReader();
        while (reader.ReadRecordHeader(out string type, out _))
        {
            var attributes = new AttributeSet();
            while (reader.ReadAttribute(out string key, out string value))
            {
                attributes.Add(key, value);
            }

            Array.Resize(ref people, people.Length + 1);
            people[^1] = _descriptors.For(type).Create(attributes);
        }

        return people;
    }

    public Person[] Find(IPersonSpecification specification) => GetAll().Filter(specification);

    public void Add(Person person)
    {
        if (Find(new IdSpecification(person.Id)).Length > 0)
        {
            throw new InvalidOperationException($"Особа з ідентифікатором «{person.Id}» уже існує.");
        }

        using IRecordWriter writer = _store.OpenAppender();
        Write(writer, person);
    }

    public bool Update(Person person) => Rewrite(person.Id, person);

    public bool Delete(string id) => Rewrite(id, null);

    /// <summary>
    /// Перезаписує файл: особа з даним ID пропускається (видалення) або замінюється. Запис іде у тимчасовий
    /// файл і підміняє оригінал лише через Commit, тож збій посеред операції не псує базу.
    /// </summary>
    private bool Rewrite(string id, Person? replacement)
    {
        Person[] all = GetAll();
        bool found = false;

        using ITransactionalRecordWriter writer = _store.OpenRewriter();
        foreach (Person person in all)
        {
            if (!string.Equals(person.Id, id, StringComparison.OrdinalIgnoreCase))
            {
                Write(writer, person);
            }
            else
            {
                found = true;
                if (replacement is not null)
                {
                    Write(writer, replacement);
                }
            }
        }

        if (found)
        {
            writer.Commit();
        }

        return found;
    }

    /// <summary>Записує особу атрибут за атрибутом (а не об'єктом цілком).</summary>
    private void Write(IRecordWriter writer, Person person)
    {
        IPersonDescriptor descriptor = _descriptors.For(person);
        AttributeSet attributes = descriptor.Describe(person);

        writer.BeginRecord(descriptor.TypeName, person.FirstName + person.LastName);
        for (int i = 0; i < attributes.Count; i++)
        {
            writer.WriteAttribute(attributes.KeyAt(i), attributes.ValueAt(i));
        }

        writer.EndRecord();
    }
}
