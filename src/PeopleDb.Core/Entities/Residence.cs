using System;
using System.Globalization;
using System.Text.RegularExpressions;
using PeopleDb.Core.Validation;

namespace PeopleDb.Core.Entities;

/// <summary>Місце проживання студента: гуртожиток або звичайна адреса (поліморфізм замість if/else за типом).</summary>
public interface IResidence
{
    bool IsDormitory { get; }

    /// <summary>Текст для файлу: «номер.кімната» для гуртожитку або адреса.</summary>
    string ToStorageString();
}

/// <summary>Гуртожиток у форматі «номер гуртожитку.кімната».</summary>
public sealed class Dormitory : IResidence
{
    public Dormitory(int building, int room)
    {
        if (building < 1 || room < 1)
        {
            throw new ArgumentException("Номер гуртожитку та кімнати мають бути додатними.");
        }

        Building = building;
        Room = room;
    }

    public int Building { get; }

    public int Room { get; }

    public bool IsDormitory => true;

    public string ToStorageString() => string.Create(CultureInfo.InvariantCulture, $"{Building}.{Room}");
}

/// <summary>Проживання поза гуртожитком.</summary>
public sealed class PrivateAddress : IResidence
{
    public PrivateAddress(string address) => Address = Rules.Require(address, Rules.Address, "Адреса");

    public string Address { get; }

    public bool IsDormitory => false;

    public string ToStorageString() => Address;
}

/// <summary>Створює потрібну реалізацію <see cref="IResidence"/> за введеним текстом.</summary>
public static class ResidenceParser
{
    public static IResidence Parse(string text)
    {
        string value = Rules.Require(text, Rules.Residence, "Місце проживання");
        if (!Regex.IsMatch(value, Rules.Dormitory))
        {
            return new PrivateAddress(value);
        }

        int dot = value.IndexOf('.');
        return new Dormitory(
            int.Parse(value.AsSpan(0, dot), NumberStyles.None, CultureInfo.InvariantCulture),
            int.Parse(value.AsSpan(dot + 1), NumberStyles.None, CultureInfo.InvariantCulture));
    }
}
