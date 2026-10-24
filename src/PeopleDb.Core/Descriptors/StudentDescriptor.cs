using PeopleDb.Core.Abilities;
using PeopleDb.Core.Entities;
using PeopleDb.Core.Validation;

namespace PeopleDb.Core.Descriptors;

public sealed class StudentDescriptor : PersonDescriptor<Student>
{
    public override string Title => "Студент";

    protected override FieldSpec[] SpecificFields { get; } =
    {
        new("studentId", "Студентський квиток", Rules.StudentId, "2 великі літери та 6 цифр, напр. КВ123456"),
        new("course", "Курс", Rules.Course, "цифра від 1 до 6"),
        new("gender", "Стать (Ч/Ж)", Rules.Gender, "літера Ч або Ж"),
        new("residence", "Місце проживання (гуртожиток «номер.кімната», напр. 5.312, або адреса)",
            Rules.Residence, "формат «номер.кімната» (5.312) або адреса з літерами")
    };

    protected override void AddSpecific(Student student, AttributeSet attributes) => attributes
        .Add("studentId", student.StudentId)
        .Add("course", student.Course)
        .Add("gender", student.Gender.ToCode())
        .Add("residence", student.Residence.ToStorageString())
        .Add("lessons", student.Lessons)
        .Add("skatingSessions", student.SkatingSessions);

    protected override Student Build(string firstName, string lastName, AttributeSet attributes) => new(
        firstName,
        lastName,
        attributes.Require("studentId"),
        attributes.RequireInt("course"),
        GenderExtensions.Parse(attributes.Require("gender")),
        ResidenceParser.Parse(attributes.Require("residence")),
        attributes.GetInt("lessons", 0),
        new SkatingSkill(attributes.GetInt("skatingSessions", 0)));
}
