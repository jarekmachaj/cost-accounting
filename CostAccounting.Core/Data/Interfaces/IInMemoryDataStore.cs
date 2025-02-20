using CostAccounting.Core.Entities;

namespace CostAccounting.Core.Data.Interfaces;
public interface IInMemoryDataStore<T, TKey>
    where T : class, IEntity<TKey>, new()
    where TKey : struct
{
    Dictionary<TKey, T> GetStore();
    T? GetById(TKey id);
    IEnumerable<T> GetAll();
    T Add(T entity);
    T AddOrUpdate(T entity);
    void Delete(T entity);
}
