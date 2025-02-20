using CostAccounting.Core.Data.Interfaces;
using CostAccounting.Core.Entities;

namespace CostAccounting.Core.Data;

public class GenericRepository<T, TKey> : IGenericRepository<T, TKey>
    where T : class, IEntity<TKey>, new()
    where TKey : struct
{
    private readonly IInMemoryDataStore<T, TKey> _dataStore;

    public GenericRepository(IInMemoryDataStore<T, TKey> dataStore)
    {
        _dataStore = dataStore;
    }

    public T Add(T entity)
    {
        return _dataStore.Add(entity);
    }

    public void Add(IList<T> entities)
    {
        foreach (var entity in entities)
        {
            _dataStore.Add(entity);
        }
    }

    public T AddOrUpdate(T entity)
    {
        return _dataStore.AddOrUpdate(entity);
    }
    public void Delete(T entity)
    {
        _dataStore.Delete(entity);
    }
    public void Edit(T entity)
    {
        _dataStore.AddOrUpdate(entity);
    }

    public Task<IEnumerable<T>> GetAllAsync()
    {
        var allEntities = _dataStore.GetAll().ToList();
        return Task.FromResult<IEnumerable<T>>(allEntities);
    }

    public Task<IEnumerable<T>> GetFilteredAsync(Func<T, bool> predicate)
    {
        var filteredEntities = _dataStore.GetAll().Where(predicate).ToList();
        return Task.FromResult<IEnumerable<T>>(filteredEntities);
    }
}