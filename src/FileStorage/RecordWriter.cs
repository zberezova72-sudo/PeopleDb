using System;
using System.IO;
using System.Text;

namespace FileStorage;

/// <summary>
/// Записує дані у форматі «Тип Назва { "ключ": "значення", ... };» через <see cref="TextWriter"/> (StreamWriter).
/// Кожен атрибут записується окремо (поелементно).
/// </summary>
public sealed class RecordWriter : IRecordWriter
{
    private readonly TextWriter _writer;
    private bool _recordOpen;
    private int _attributeCount;

    public RecordWriter(TextWriter writer)
    {
        ArgumentNullException.ThrowIfNull(writer);
        _writer = writer;
    }

    public void BeginRecord(string type, string name)
    {
        if (_recordOpen)
        {
            throw new InvalidOperationException("Попередній запис не завершено викликом EndRecord.");
        }

        RequireToken(type, nameof(type));
        RequireToken(name, nameof(name));

        _writer.WriteLine($"{type} {name}");
        _writer.Write("{ ");
        _recordOpen = true;
        _attributeCount = 0;
    }

    public void WriteAttribute(string key, string value)
    {
        if (!_recordOpen)
        {
            throw new InvalidOperationException("Спочатку потрібно викликати BeginRecord.");
        }

        ArgumentException.ThrowIfNullOrEmpty(key);
        ArgumentNullException.ThrowIfNull(value);

        if (_attributeCount > 0)
        {
            _writer.WriteLine(',');
        }

        _writer.Write(Quote(key));
        _writer.Write(": ");
        _writer.Write(Quote(value));
        _attributeCount++;
    }

    public void EndRecord()
    {
        if (!_recordOpen)
        {
            throw new InvalidOperationException("Немає відкритого запису для завершення.");
        }

        _writer.WriteLine("};");
        _recordOpen = false;
    }

    public void Dispose() => _writer.Dispose();

    /// <summary>Тип і назва об'єкта — окремі «слова» заголовка, тому не можуть бути порожніми чи містити пробіли.</summary>
    private static void RequireToken(string token, string parameter)
    {
        if (string.IsNullOrEmpty(token) || token.AsSpan().IndexOfAny(" \t\r\n") >= 0)
        {
            throw new ArgumentException("Значення має бути непорожнім і без пробілів.", parameter);
        }
    }

    /// <summary>Береться в лапки; лапки, зворотний слеш та переноси рядків екрануються.</summary>
    private static string Quote(string text)
    {
        var builder = new StringBuilder(text.Length + 2).Append('"');
        foreach (char c in text)
        {
            switch (c)
            {
                case '"': builder.Append("\\\""); break;
                case '\\': builder.Append("\\\\"); break;
                case '\n': builder.Append("\\n"); break;
                case '\r': builder.Append("\\r"); break;
                default: builder.Append(c); break;
            }
        }

        return builder.Append('"').ToString();
    }
}
