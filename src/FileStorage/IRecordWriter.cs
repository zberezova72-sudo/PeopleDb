using System;

namespace FileStorage;

/// <summary>Послідовний запис у файл поелементно: заголовок, атрибути, завершення запису.</summary>
public interface IRecordWriter : IDisposable
{
    /// <summary>Починає новий запис «ТипДаних НазваОб'єкта».</summary>
    void BeginRecord(string type, string name);

    /// <summary>Записує один атрибут «ключ»: «значення» поточного запису.</summary>
    void WriteAttribute(string key, string value);

    /// <summary>Завершує поточний запис.</summary>
    void EndRecord();
}

/// <summary>
/// Записувач, що пише у тимчасовий файл. Зміни потрапляють у справжній файл
/// лише після <see cref="Commit"/>; якщо Commit не викликано — вони відкидаються.
/// </summary>
public interface ITransactionalRecordWriter : IRecordWriter
{
    /// <summary>Підміняє цільовий файл тимчасовим. Викликати після закриття читача цього файлу.</summary>
    void Commit();
}
