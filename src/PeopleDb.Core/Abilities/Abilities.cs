namespace PeopleDb.Core.Abilities;

// Кожне вміння — окремий вузький інтерфейс (Interface Segregation). Сутність підтримує вміння,
// просто реалізуючи інтерфейс; наявні типи при цьому не змінюються. Методи змінюють стан об'єкта,
// а не друкують повідомлення — вивід робить лише ConsoleMenu.

/// <summary>Навчання (у ВНЗ — для студента).</summary>
public interface ILearner
{
    int Lessons { get; }
    void Study();
}

/// <summary>Гра на інструменті.</summary>
public interface IPlayer
{
    int PiecesPlayed { get; }
    void Play();
}

/// <summary>Польоти.</summary>
public interface IFlyer
{
    int FlightHours { get; }
    void Fly();
}

/// <summary>Додаткове вміння за варіантом №3: кататися на ковзанах.</summary>
public interface ISkater
{
    int SkatingSessions { get; }
    void Skate();
}
