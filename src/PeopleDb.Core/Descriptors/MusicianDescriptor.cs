using PeopleDb.Core.Abilities;
using PeopleDb.Core.Entities;
using PeopleDb.Core.Validation;

namespace PeopleDb.Core.Descriptors;

public sealed class MusicianDescriptor : PersonDescriptor<Musician>
{
    public override string Title => "Музикант";

    protected override FieldSpec[] SpecificFields { get; } =
    {
        new("musicianId", "Посвідчення музиканта", Rules.MusicianId, "MU та 6 цифр, напр. MU100001"),
        new("instrument", "Інструмент", Rules.Instrument, "літери, пробіли, апостроф або дефіс")
    };

    protected override void AddSpecific(Musician musician, AttributeSet attributes) => attributes
        .Add("musicianId", musician.MusicianId)
        .Add("instrument", musician.Instrument)
        .Add("piecesPlayed", musician.PiecesPlayed)
        .Add("skatingSessions", musician.SkatingSessions);

    protected override Musician Build(string firstName, string lastName, AttributeSet attributes) => new(
        firstName,
        lastName,
        attributes.Require("musicianId"),
        attributes.Require("instrument"),
        attributes.GetInt("piecesPlayed", 0),
        new SkatingSkill(attributes.GetInt("skatingSessions", 0)));
}
