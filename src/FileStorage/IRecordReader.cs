using System;

namespace FileStorage;

/// <summary>
/// Послідовне (потокове) читання записів файлу поелементно:
/// спочатку заголовок запису, потім його атрибути один за одним.
/// Нічого не знає про предметну область — лише про формат файлу.
/// </summary>
public interface IRecordReader : IDisposable
{
    /// <summary>Переходить до наступного запису. Повертає false, якщо записів більше немає.</summary>
    bool ReadRecordHeader(out string type, out string name);

    /// <summary>Читає наступний атрибут поточного запису. Повертає false, коли запис закінчився.</summary>
    bool ReadAttribute(out string key, out string value);
}
