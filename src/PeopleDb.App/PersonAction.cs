using System;
using PeopleDb.Core.Entities;

namespace PeopleDb.App;

/// <summary>
/// Дія, яку можна виконати над особою. Меню показує лише ті дії, що застосовні до обраної особи,
/// тож нові вміння додаються без змін меню та сутностей.
/// </summary>
public interface IPersonAction
{
    string Title { get; }

    bool AppliesTo(Person person);

    void Execute(Person person);
}

/// <summary>Універсальна дія для будь-якого вміння (інтерфейсу): застосовна до осіб, що його реалізують.</summary>
public sealed class PersonAction<TAbility> : IPersonAction where TAbility : class
{
    private readonly Action<TAbility> _run;

    public PersonAction(string title, Action<TAbility> run)
    {
        Title = title;
        _run = run;
    }

    public string Title { get; }

    public bool AppliesTo(Person person) => person is TAbility;

    public void Execute(Person person) => _run((TAbility)(object)person);
}
