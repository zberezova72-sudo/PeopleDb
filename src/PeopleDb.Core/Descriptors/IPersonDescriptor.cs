using PeopleDb.Core.Entities;

namespace PeopleDb.Core.Descriptors;

/// <summary>
/// Відображає один тип особи на набір атрибутів і назад. Щоб додати новий тип (наприклад, Pupil),
/// достатньо створити сутність і дескриптор та передати його в <see cref="DescriptorRegistry"/>
/// у корені композиції — репозиторій, меню та формат файлу змінювати не потрібно (Open/Closed).
/// </summary>
public interface IPersonDescriptor
{
    /// <summary>Ім'я типу у файлі («Student», «Musician», …).</summary>
    string TypeName { get; }

    /// <summary>Назва для відображення користувачу.</summary>
    string Title { get; }

    /// <summary>Поля, які користувач вводить при додаванні особи цього типу.</summary>
    FieldSpec[] Fields { get; }

    bool Handles(Person person);

    AttributeSet Describe(Person person);

    Person Create(AttributeSet attributes);
}
