using PeopleDb.Core.Validation;

namespace PeopleDb.Core.Entities;

public enum Gender
{
    Male,
    Female
}

public static class GenderExtensions
{
    /// <summary>Розбирає «Ч»/«Ж» (регістр не важливий).</summary>
    public static Gender Parse(string text) =>
        char.ToUpperInvariant(Rules.Require(text, Rules.Gender, "Стать")[0]) == 'Ж' ? Gender.Female : Gender.Male;

    /// <summary>Код для збереження у файлі.</summary>
    public static string ToCode(this Gender gender) => gender == Gender.Female ? "Ж" : "Ч";
}
