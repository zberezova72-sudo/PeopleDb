namespace FileStorage;

/// <summary>
/// Абстракція сховища записів. Клієнти залежать від неї, а не від конкретного файлу
/// (інверсія залежностей) — сховище можна підмінити без змін решти коду.
/// </summary>
public interface IRecordStore
{
    /// <summary>Відкриває сховище для читання (відсутній файл трактується як порожнє сховище).</summary>
    IRecordReader OpenReader();

    /// <summary>Відкриває сховище для дописування записів у кінець.</summary>
    IRecordWriter OpenAppender();

    /// <summary>Відкриває запис «з нуля» у тимчасовий файл (для видалення/оновлення записів).</summary>
    ITransactionalRecordWriter OpenRewriter();
}
