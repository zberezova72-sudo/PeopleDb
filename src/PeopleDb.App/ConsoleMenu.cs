using System;
using System.IO;
using System.Text.RegularExpressions;
using PeopleDb.Core.Descriptors;
using PeopleDb.Core.Entities;
using PeopleDb.Core.Persistence;
using PeopleDb.Core.Specifications;
using PeopleDb.Core.Validation;

namespace PeopleDb.App;

/// <summary>
/// Уся взаємодія з користувачем (ввід та вивід на екран) зосереджена тут. Клас залежить лише від абстракцій
/// (репозиторій, реєстр дескрипторів, дії), а потоки вводу/виводу отримує ззовні.
/// </summary>
public sealed class ConsoleMenu
{
    private readonly IPersonRepository _repository;
    private readonly DescriptorRegistry _descriptors;
    private readonly IPersonAction[] _actions;
    private readonly DemoDataLoader _demoData;
    private readonly TextReader _input;
    private readonly TextWriter _output;

    public ConsoleMenu(
        IPersonRepository repository,
        DescriptorRegistry descriptors,
        IPersonAction[] actions,
        DemoDataLoader demoData,
        TextReader input,
        TextWriter output)
    {
        _repository = repository;
        _descriptors = descriptors;
        _actions = actions;
        _demoData = demoData;
        _input = input;
        _output = output;
    }

    public void Run()
    {
        try
        {
            while (true)
            {
                ShowMenu();
                int choice = ReadNumber("Ваш вибір", 0, 8);
                if (choice == 0)
                {
                    _output.WriteLine("До побачення!");
                    return;
                }

                Execute(choice);
            }
        }
        catch (OperationCanceledException)
        {
            // Потік вводу закінчився (Ctrl+D / Ctrl+Z або порожній stdin) — завершуємо роботу.
            _output.WriteLine();
            _output.WriteLine("Ввід завершено.");
        }
    }

    private void ShowMenu()
    {
        _output.WriteLine();
        _output.WriteLine("=========== БАЗА ДАНИХ ОСІБ ===========");
        _output.WriteLine("1. Завантажити демонстраційні дані");
        _output.WriteLine("2. Додати N осіб");
        _output.WriteLine("3. Показати всіх осіб (читання з файлу)");
        _output.WriteLine("4. Пошук за прізвищем");
        _output.WriteLine("5. Пошук за унікальним ідентифікатором");
        _output.WriteLine("6. Видалити особу за ідентифікатором");
        _output.WriteLine("7. Виконати дію над особою (навчання, гра, політ, катання на ковзанах)");
        _output.WriteLine("8. Варіант №3: кількість студенток 1-го курсу в гуртожитку");
        _output.WriteLine("0. Вихід");
    }

    private void Execute(int choice)
    {
        try
        {
            switch (choice)
            {
                case 1: LoadDemoData(); break;
                case 2: AddPeople(); break;
                case 3: ShowAll(); break;
                case 4: SearchByLastName(); break;
                case 5: SearchById(); break;
                case 6: DeleteById(); break;
                case 7: PerformAction(); break;
                case 8: ShowFirstYearFemalesInDormitory(); break;
            }
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidDataException or InvalidOperationException
                                       or FormatException or IOException)
        {
            _output.WriteLine($"Помилка: {ex.Message}");
        }
    }

    // ---------- Пункти меню ----------

    private void LoadDemoData()
    {
        int added = _demoData.Load();
        _output.WriteLine($"Додано осіб: {added} (наявні в базі пропущено).");
    }

    private void AddPeople()
    {
        int count = ReadNumber("Скільки осіб додати", 1, 50);
        for (int i = 1; i <= count; i++)
        {
            _output.WriteLine($"--- Особа {i} з {count} ---");
            for (int t = 0; t < _descriptors.Count; t++)
            {
                _output.WriteLine($"  {t + 1}. {_descriptors.At(t).Title}");
            }

            IPersonDescriptor descriptor = _descriptors.At(ReadNumber("Тип особи", 1, _descriptors.Count) - 1);

            // Дані вводяться поелементно: по одному полю з перевіркою регулярним виразом.
            var attributes = new AttributeSet();
            foreach (FieldSpec field in descriptor.Fields)
            {
                attributes.Add(field.Key, ReadValidated(field.Prompt, field.Pattern, field.Hint));
            }

            Person person = descriptor.Create(attributes);
            _repository.Add(person);
            _output.WriteLine("Збережено.");
        }
    }

    private void ShowAll()
    {
        PrintAll(_repository.GetAll(), "База порожня.");
    }

    private void SearchByLastName()
    {
        string lastName = ReadValidated("Прізвище", Rules.Name, "літери, апостроф або дефіс");
        PrintAll(_repository.Find(new LastNameSpecification(lastName)), "Нікого не знайдено.");
    }

    private void SearchById()
    {
        string id = ReadValidated("Ідентифікатор", @"^\S{1,20}$", "до 20 символів без пробілів");
        PrintAll(_repository.Find(new IdSpecification(id)), "Нікого не знайдено.");
    }

    private void DeleteById()
    {
        string id = ReadValidated("Ідентифікатор для видалення", @"^\S{1,20}$", "до 20 символів без пробілів");
        _output.WriteLine(_repository.Delete(id) ? "Особу видалено." : "Особу з таким ідентифікатором не знайдено.");
    }

    private void PerformAction()
    {
        string id = ReadValidated("Ідентифікатор особи", @"^\S{1,20}$", "до 20 символів без пробілів");
        Person[] found = _repository.Find(new IdSpecification(id));
        if (found.Length == 0)
        {
            _output.WriteLine("Особу не знайдено.");
            return;
        }

        Person person = found[0];

        // Збираємо дії, застосовні до цієї особи (визначається її вміннями).
        IPersonAction[] applicable = new IPersonAction[_actions.Length];
        int count = 0;
        foreach (IPersonAction action in _actions)
        {
            if (action.AppliesTo(person))
            {
                applicable[count++] = action;
                _output.WriteLine($"  {count}. {action.Title}");
            }
        }

        if (count == 0)
        {
            _output.WriteLine("Для цієї особи немає доступних дій.");
            return;
        }

        applicable[ReadNumber("Дія", 1, count) - 1].Execute(person);
        _repository.Update(person);
        _output.WriteLine("Дію виконано, стан збережено:");
        Print(person);
    }

    private void ShowFirstYearFemalesInDormitory()
    {
        // Дані беруться безпосередньо з файлу; відбір виконує специфікація.
        Person[] students = _repository.Find(new FirstYearFemaleInDormitorySpecification());
        _output.WriteLine($"Студенток 1-го курсу, які проживають у гуртожитку: {students.Length}");
        foreach (Person student in students)
        {
            Print(student);
        }
    }

    // ---------- Вивід ----------

    private void PrintAll(Person[] people, string emptyMessage)
    {
        if (people.Length == 0)
        {
            _output.WriteLine(emptyMessage);
            return;
        }

        _output.WriteLine($"Знайдено осіб: {people.Length}");
        foreach (Person person in people)
        {
            Print(person);
        }
    }

    /// <summary>Виводить особу поелементно (атрибут за атрибутом).</summary>
    private void Print(Person person)
    {
        IPersonDescriptor descriptor = _descriptors.For(person);
        AttributeSet attributes = descriptor.Describe(person);

        _output.WriteLine($"[{descriptor.Title}]");
        for (int i = 0; i < attributes.Count; i++)
        {
            _output.WriteLine($"    {attributes.KeyAt(i)}: {attributes.ValueAt(i)}");
        }
    }

    // ---------- Ввід ----------

    private string ReadLine()
    {
        string? line = _input.ReadLine();
        return line is null ? throw new OperationCanceledException() : line.Trim();
    }

    /// <summary>Запитує значення, доки воно не відповідатиме регулярному виразу.</summary>
    private string ReadValidated(string prompt, string pattern, string hint)
    {
        while (true)
        {
            _output.Write($"{prompt}: ");
            string value = ReadLine();
            if (Regex.IsMatch(value, pattern))
            {
                return value;
            }

            _output.WriteLine($"  Некоректне значення. Очікується: {hint}.");
        }
    }

    private int ReadNumber(string prompt, int min, int max)
    {
        string text = ReadValidated($"{prompt} [{min}-{max}]", @"^\d{1,4}$", "число");
        int number = int.Parse(text);
        if (number >= min && number <= max)
        {
            return number;
        }

        _output.WriteLine($"  Число має бути від {min} до {max}.");
        return ReadNumber(prompt, min, max);
    }
}
