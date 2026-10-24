using System;
using System.IO;
using System.Text;

namespace FileStorage;

/// <summary>
/// Читає формат:
/// <code>
/// Тип НазваОб'єкта
/// { "атрибут1": "значення1",
/// "атрибут2": "значення2"};
/// </code>
/// Читання посимвольне через <see cref="TextReader"/> (StreamReader), без завантаження всього файлу.
/// Допускаються як прямі ("), так і типографські (“ ”, « ») лапки.
/// </summary>
public sealed class RecordReader : IRecordReader
{
    private readonly TextReader _reader;
    private int _line = 1;
    private bool _insideRecord;

    public RecordReader(TextReader reader)
    {
        ArgumentNullException.ThrowIfNull(reader);
        _reader = reader;
    }

    public bool ReadRecordHeader(out string type, out string name)
    {
        // Дочитуємо невикористані атрибути попереднього запису.
        while (ReadAttribute(out _, out _))
        {
        }

        SkipWhitespace();
        if (_reader.Peek() < 0)
        {
            type = name = string.Empty;
            return false;
        }

        type = ReadToken("тип даних");
        SkipSpaces();
        name = ReadToken("назву об'єкта");
        SkipWhitespace();
        Expect('{');
        _insideRecord = true;
        return true;
    }

    public bool ReadAttribute(out string key, out string value)
    {
        key = value = string.Empty;
        if (!_insideRecord)
        {
            return false;
        }

        SkipSeparators();
        if (_reader.Peek() == '}')
        {
            Next();
            SkipSpaces();
            if (_reader.Peek() == ';')
            {
                Next();
            }

            _insideRecord = false;
            return false;
        }

        key = ReadQuoted();
        SkipWhitespace();
        Expect(':');
        SkipWhitespace();
        value = ReadQuoted();
        return true;
    }

    public void Dispose() => _reader.Dispose();

    // ---------- Допоміжні методи лексичного розбору ----------

    /// <summary>Зчитує символ і веде облік номера рядка для зрозумілих повідомлень про помилки.</summary>
    private int Next()
    {
        int c = _reader.Read();
        if (c == '\n')
        {
            _line++;
        }

        return c;
    }

    private void SkipWhitespace()
    {
        while (_reader.Peek() >= 0 && char.IsWhiteSpace((char)_reader.Peek()))
        {
            Next();
        }
    }

    private void SkipSpaces()
    {
        while (_reader.Peek() == ' ' || _reader.Peek() == '\t')
        {
            Next();
        }
    }

    /// <summary>Пропускає пробіли, переноси рядків та коми між атрибутами.</summary>
    private void SkipSeparators()
    {
        while (_reader.Peek() >= 0 && (char.IsWhiteSpace((char)_reader.Peek()) || _reader.Peek() == ','))
        {
            Next();
        }
    }

    private string ReadToken(string what)
    {
        var builder = new StringBuilder();
        while (_reader.Peek() >= 0 && !char.IsWhiteSpace((char)_reader.Peek()))
        {
            builder.Append((char)Next());
        }

        if (builder.Length == 0)
        {
            throw Error($"очікувалось {what}");
        }

        return builder.ToString();
    }

    private string ReadQuoted()
    {
        if (!IsQuote(Next()))
        {
            throw Error("очікувалась відкриваюча лапка");
        }

        var builder = new StringBuilder();
        while (true)
        {
            int c = Next();
            if (c < 0)
            {
                throw Error("значення не закрите лапкою");
            }

            if (IsQuote(c))
            {
                return builder.ToString();
            }

            if (c == '\\')
            {
                int escaped = Next();
                if (escaped < 0)
                {
                    throw Error("несподіваний кінець файлу після '\\'");
                }

                builder.Append(escaped switch { 'n' => '\n', 'r' => '\r', _ => (char)escaped });
                continue;
            }

            builder.Append((char)c);
        }
    }

    private static bool IsQuote(int c) => c is '"' or '“' or '”' or '«' or '»';

    private void Expect(char expected)
    {
        if (Next() != expected)
        {
            throw Error($"очікувався символ '{expected}'");
        }
    }

    private FormatException Error(string message) => new($"Рядок {_line}: {message}.");
}
