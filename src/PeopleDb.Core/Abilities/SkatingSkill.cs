using PeopleDb.Core.Validation;

namespace PeopleDb.Core.Abilities;

/// <summary>
/// Компонент «катання на ковзанах». Сутності (Student, Musician, Pilot) не наслідують цю поведінку,
/// а отримують її композицією та делегують виклики (composition over inheritance).
/// </summary>
public sealed class SkatingSkill : ISkater
{
    public SkatingSkill(int sessions = 0) => SkatingSessions = Rules.RequireNonNegative(sessions, "Катання на ковзанах");

    public int SkatingSessions { get; private set; }

    public void Skate() => SkatingSessions++;
}
