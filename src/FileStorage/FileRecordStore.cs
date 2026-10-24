using System;
using System.IO;
using System.Text;

namespace FileStorage;

/// <summary>
/// Сховище записів в одному текстовому файлі (UTF-8).
/// Використовує потоки: FileStream → StreamReader / StreamWriter.
/// </summary>
public sealed class FileRecordStore : IRecordStore
{
    private static readonly Encoding Utf8 = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

    private readonly string _path;

    public FileRecordStore(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        _path = Path.GetFullPath(path);
    }

    public IRecordReader OpenReader()
    {
        if (!File.Exists(_path))
        {
            return new RecordReader(TextReader.Null);
        }

        var stream = new FileStream(_path, FileMode.Open, FileAccess.Read, FileShare.Read);
        return new RecordReader(new StreamReader(stream, Utf8));
    }

    public IRecordWriter OpenAppender()
    {
        EnsureDirectory();
        var stream = new FileStream(_path, FileMode.Append, FileAccess.Write, FileShare.Read);
        return new RecordWriter(new StreamWriter(stream, Utf8));
    }

    public ITransactionalRecordWriter OpenRewriter()
    {
        EnsureDirectory();
        return new RewriteSession(_path, Utf8);
    }

    private void EnsureDirectory()
    {
        string? directory = Path.GetDirectoryName(_path);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }
}
