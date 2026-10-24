using System;
using System.IO;
using PeopleDb.Core.Entities;

namespace PeopleDb.Core.Descriptors;

/// <summary>Реєстр дескрипторів типів. Отримує їх ззовні (dependency injection), тож сам не знає конкретних типів.</summary>
public sealed class DescriptorRegistry
{
    private readonly IPersonDescriptor[] _descriptors;

    public DescriptorRegistry(params IPersonDescriptor[] descriptors) => _descriptors = descriptors;

    public int Count => _descriptors.Length;

    public IPersonDescriptor At(int index) => _descriptors[index];

    /// <summary>Дескриптор за іменем типу з файлу (невідомий тип — помилка даних).</summary>
    public IPersonDescriptor For(string typeName)
    {
        foreach (IPersonDescriptor descriptor in _descriptors)
        {
            if (string.Equals(descriptor.TypeName, typeName, StringComparison.OrdinalIgnoreCase))
            {
                return descriptor;
            }
        }

        throw new InvalidDataException($"Невідомий тип даних «{typeName}».");
    }

    /// <summary>Дескриптор для об'єкта особи.</summary>
    public IPersonDescriptor For(Person person)
    {
        foreach (IPersonDescriptor descriptor in _descriptors)
        {
            if (descriptor.Handles(person))
            {
                return descriptor;
            }
        }

        throw new InvalidOperationException($"Тип {person.GetType().Name} не зареєстровано.");
    }
}
