using System;
using PeopleDb.Core.Abilities;
using PeopleDb.Core.Validation;

namespace PeopleDb.Core.Entities;

/// <summary>Студент: прізвище, ім'я, курс, студентський квиток, стать, місце проживання. Вміння: Study, Skate.</summary>
public sealed class Student : Person, ILearner, ISkater
{
    private readonly ISkater _skating;

    public Student(
        string firstName,
        string lastName,
        string studentId,
        int course,
        Gender gender,
        IResidence residence,
        int lessons = 0,
        ISkater? skating = null) : base(firstName, lastName)
    {
        StudentId = Rules.Require(studentId, Rules.StudentId, "Студентський квиток");
        Course = course >= 1 && course <= 6
            ? course
            : throw new ArgumentException($"Некоректне значення поля «Курс»: '{course}'.", nameof(course));
        Gender = gender;
        Residence = residence ?? throw new ArgumentNullException(nameof(residence));
        Lessons = Rules.RequireNonNegative(lessons, "Заняття");
        _skating = skating ?? new SkatingSkill();
    }

    public string StudentId { get; }

    public int Course { get; }

    public Gender Gender { get; }

    public IResidence Residence { get; }

    public int Lessons { get; private set; }

    public override string Id => StudentId;

    public int SkatingSessions => _skating.SkatingSessions;

    /// <summary>Навчання у ВНЗ: збільшує кількість відвіданих занять.</summary>
    public void Study() => Lessons++;

    public void Skate() => _skating.Skate();
}
