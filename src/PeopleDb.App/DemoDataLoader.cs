using PeopleDb.Core.Abilities;
using PeopleDb.Core.Entities;
using PeopleDb.Core.Persistence;
using PeopleDb.Core.Specifications;

namespace PeopleDb.App;

/// <summary>Набір вхідних даних для демонстрації (12 осіб трьох типів). Консоль не використовує.</summary>
public sealed class DemoDataLoader
{
    private readonly IPersonRepository _repository;

    public DemoDataLoader(IPersonRepository repository) => _repository = repository;

    public Person[] Create() => new Person[]
    {
        // Студентки 1-го курсу в гуртожитку — результат запиту варіанту: 3.
        new Student("Оксана", "Коваленко", "КВ123456", 1, Gender.Female, new Dormitory(5, 312)),
        new Student("Марія", "Шевченко", "КВ123457", 1, Gender.Female, new Dormitory(5, 101)),
        new Student("Наталія", "Кравчук", "КВ123461", 1, Gender.Female, new Dormitory(3, 118)),
        // Не підходять: живе вдома / чоловіча стать / 2-й курс.
        new Student("Ірина", "Бондаренко", "КВ123458", 1, Gender.Female, new PrivateAddress("м. Київ, вул. Шевченка 10")),
        new Student("Андрій", "Мельник", "КВ123459", 1, Gender.Male, new Dormitory(5, 204)),
        new Student("Софія", "Ткаченко", "КВ223460", 2, Gender.Female, new Dormitory(3, 45)),
        new Student("Богдан", "Коваленко", "КВ323462", 3, Gender.Male, new Dormitory(2, 77), lessons: 12),
        new Musician("Тарас", "Лисенко", "MU100001", "гітара", piecesPlayed: 5),
        new Musician("Леся", "Мороз", "MU100002", "скрипка"),
        new Pilot("Віктор", "Гончар", "PL-200001", 1200, new SkatingSkill(2)),
        new Pilot("Олена", "Савчук", "PL-200002", 350),
        new Pilot("Максим", "Мороз", "PL-200003", 80)
    };

    /// <summary>Додає демонстраційних осіб, яких ще немає в базі. Повертає кількість доданих.</summary>
    public int Load()
    {
        int added = 0;
        foreach (Person person in Create())
        {
            if (_repository.Find(new IdSpecification(person.Id)).Length == 0)
            {
                _repository.Add(person);
                added++;
            }
        }

        return added;
    }
}
