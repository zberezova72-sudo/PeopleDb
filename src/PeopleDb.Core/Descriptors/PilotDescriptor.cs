using PeopleDb.Core.Abilities;
using PeopleDb.Core.Entities;
using PeopleDb.Core.Validation;

namespace PeopleDb.Core.Descriptors;

public sealed class PilotDescriptor : PersonDescriptor<Pilot>
{
    public override string Title => "Пілот";

    protected override FieldSpec[] SpecificFields { get; } =
    {
        new("licenseNumber", "Номер ліцензії", Rules.LicenseNumber, "PL- та 6 цифр, напр. PL-200001"),
        new("flightHours", "Години нальоту", Rules.Counter, "ціле число до 6 цифр")
    };

    protected override void AddSpecific(Pilot pilot, AttributeSet attributes) => attributes
        .Add("licenseNumber", pilot.LicenseNumber)
        .Add("flightHours", pilot.FlightHours)
        .Add("skatingSessions", pilot.SkatingSessions);

    protected override Pilot Build(string firstName, string lastName, AttributeSet attributes) => new(
        firstName,
        lastName,
        attributes.Require("licenseNumber"),
        attributes.GetInt("flightHours", 0),
        new SkatingSkill(attributes.GetInt("skatingSessions", 0)));
}
