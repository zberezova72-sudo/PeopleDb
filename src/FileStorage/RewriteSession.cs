using System;
using System.IO;
using System.Text;

namespace FileStorage;

/// <summary>
/// Композиція замість наслідування: сесія перезапису «обгортає» звичайний <see cref="RecordWriter"/>,
/// спрямований у тимчасовий файл, і додає лише семантику Commit/відкату.
/// </summary>
internal sealed class RewriteSession : ITransactionalRecordWriter
{
    private readonly string _targetPath;
    private readonly string _tempPath;
    private readonly RecordWriter _inner;
    private bool _finished;

    public RewriteSession(string targetPath, Encoding encoding)
    {
        _targetPath = targetPath;
        _tempPath = targetPath + ".tmp";
        var stream = new FileStream(_tempPath, FileMode.Create, FileAccess.Write, FileShare.None);
        _inner = new RecordWriter(new StreamWriter(stream, encoding));
    }

    public void BeginRecord(string type, string name) => _inner.BeginRecord(type, name);

    public void WriteAttribute(string key, string value) => _inner.WriteAttribute(key, value);

    public void EndRecord() => _inner.EndRecord();

    public void Commit()
    {
        if (_finished)
        {
            throw new InvalidOperationException("Сесію перезапису вже завершено.");
        }

        _inner.Dispose();
        File.Move(_tempPath, _targetPath, overwrite: true);
        _finished = true;
    }

    public void Dispose()
    {
        if (_finished)
        {
            return;
        }

        // Commit не викликано — відкидаємо тимчасовий файл, оригінал лишається недоторканим.
        _inner.Dispose();
        File.Delete(_tempPath);
        _finished = true;
    }
}
