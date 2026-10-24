using PeopleDb.Core.Abilities;
using PeopleDb.Core.Validation;

namespace PeopleDb.Core.Entities;

/// <summary>Музикант. Вміння: Play, Skate.</summary>
public sealed class Musician : Person, IPlayer, ISkater
{
    private readonly ISkater _skating;

    public Musician(
        string firstName,
        string lastName,
        string musicianId,
        string instrument,
        int piecesPlayed = 0,
        ISkater? skating = null) : base(firstName, lastName)
    {
        MusicianId = Rules.Require(musicianId, Rules.MusicianId, "Посвідчення музиканта");
        Instrument = Rules.Require(instrument, Rules.Instrument, "Інструмент");
        PiecesPlayed = Rules.RequireNonNegative(piecesPlayed, "Виконані твори");
        _skating = skating ?? new SkatingSkill();
    }

    public string MusicianId { get; }

    public string Instrument { get; }

    public int PiecesPlayed { get; private set; }

    public override string Id => MusicianId;

    public int SkatingSessions => _skating.SkatingSessions;

    public void Play() => PiecesPlayed++;

    public void Skate() => _skating.Skate();
}
