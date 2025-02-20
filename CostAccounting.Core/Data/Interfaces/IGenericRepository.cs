using CostAccounting.Core.Entities;

namespace CostAccounting.Core.Data.Interfaces;

public interface IGenericRepository<T, TKey>
    where T : IEntity<TKey>
    where TKey : struct
{
    Task<IEnumerable<T>> GetAllAsync();

    Task<IEnumerable<T>> GetFilteredAsync(Func<T, bool> predicate);
    void Delete(T entity);
    void Edit(T entity);
}
