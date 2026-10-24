using System;
using System.Text;
using FileStorage;
using PeopleDb.App;
using PeopleDb.Core.Abilities;
using PeopleDb.Core.Descriptors;
using PeopleDb.Core.Persistence;

// Корінь композиції (Inversion of Control): тут єдине місце, де створюються конкретні об'єкти та
// передаються споживачам через конструктори. Решта коду залежить від абстракцій.

Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;

// Шлях до файлу бази можна передати першим аргументом; за умовчанням — people.txt у поточній теці.
string path = args.Length > 0 ? args[0] : "people.txt";

IRecordStore store = new FileRecordStore(path);

// Новий тип особи (напр. Pupil) = нова сутність + дескриптор + один рядок тут.
var descriptors = new DescriptorRegistry(
    new StudentDescriptor(),
    new MusicianDescriptor(),
    new PilotDescriptor());

// Нове вміння = новий інтерфейс + одна дія тут.
IPersonAction[] actions =
{
    new PersonAction<ILearner>("Навчатися (Study)", person => person.Study()),
    new PersonAction<IPlayer>("Грати (Play)", person => person.Play()),
    new PersonAction<IFlyer>("Літати (Fly)", person => person.Fly()),
    new PersonAction<ISkater>("Кататися на ковзанах (Skate)", person => person.Skate())
};

IPersonRepository repository = new PersonRepository(store, descriptors);
var menu = new ConsoleMenu(repository, descriptors, actions, new DemoDataLoader(repository), Console.In, Console.Out);

Console.WriteLine($"Файл бази даних: {System.IO.Path.GetFullPath(path)}");
menu.Run();
