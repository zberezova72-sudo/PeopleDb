using PeopleDb.Core.Abilities;
using PeopleDb.Core.Validation;

namespace PeopleDb.Core.Entities;

/// <summary>Пілот. Вміння: Fly, Skate.</summary>
public sealed class Pilot : Person, IFlyer, ISkater
{
    private readonly ISkater _skating;

    public Pilot(
        string firstName,
        string lastName,
        string licenseNumber,
        int flightHours = 0,
        ISkater? skating = null) : base(firstName, lastName)
    {
        LicenseNumber = Rules.Require(licenseNumber, Rules.LicenseNumber, "Номер ліцензії");
        FlightHours = Rules.RequireNonNegative(flightHours, "Години нальоту");
        _skating = skating ?? new SkatingSkill();
    }

    public string LicenseNumber { get; }

    public int FlightHours { get; private set; }

    public override string Id => LicenseNumber;

    public int SkatingSessions => _skating.SkatingSessions;

    /// <summary>Один політ = одна година нальоту.</summary>
    public void Fly() => FlightHours++;

    public void Skate() => _skating.Skate();
}
