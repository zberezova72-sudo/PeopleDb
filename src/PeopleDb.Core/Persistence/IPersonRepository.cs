using PeopleDb.Core.Entities;
using PeopleDb.Core.Specifications;

namespace PeopleDb.Core.Persistence;

/// <summary>Операції над базою осіб. Меню залежить від цієї абстракції, а не від файлів.</summary>
public interface IPersonRepository
{
    Person[] GetAll();

    /// <summary>Повертає особи, що задовольняють умову (пошук за прізвищем, за ID, запит варіанту тощо).</summary>
    Person[] Find(IPersonSpecification specification);

    /// <summary>Додає особу; якщо ідентифікатор уже існує — InvalidOperationException.</summary>
    void Add(Person person);

    /// <summary>Замінює збережену особу з тим самим ідентифікатором. false — такої особи немає.</summary>
    bool Update(Person person);

    /// <summary>Видаляє особу за ідентифікатором. false — такої особи немає.</summary>
    bool Delete(string id);
}
