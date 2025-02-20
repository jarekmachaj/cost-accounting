using CostAccounting.Core.Data.Interfaces;
using CostAccounting.Core.Entities;
using System.Reflection;

namespace CostAccounting.Core.Data;
public class InMemoryDataStore<T, TKey> : IInMemoryDataStore<T, TKey>
    where T : class, IEntity<TKey>, new()
    where TKey : struct
{
    private readonly Dictionary<TKey, T> _store = new();

    public Dictionary<TKey, T> GetStore() => _store;

    public T? GetById(TKey id)
    {
        if (_store.TryGetValue(id, out var entity))
        {
            return Clone(entity);
        }
        return null;
    }

    public IEnumerable<T> GetAll() => _store.Values.Select(Clone).ToList();

    public T Add(T entity)
    {
        // Auto-generate an Id if needed (example for int keys).
        if (entity.Id.Equals(default(TKey)))
        {
            if (typeof(TKey) == typeof(int))
            {
                int nextId = _store.Any() ? _store.Keys.Select(k => (int)(object)k).Max() + 1 : 1;
                entity.Id = (TKey)(object)nextId;
            }
            else if (typeof(TKey) == typeof(Guid))
            {
                entity.Id = (TKey)(object)Guid.NewGuid();
            }
            else
            {
                throw new InvalidOperationException("Keys must be set explicitly.");
            }
        }
        _store[entity.Id] = Clone(entity);
        return entity;
    }

    public T AddOrUpdate(T entity)
    {
        _store[entity.Id] = Clone(entity);
        return entity;
    }

    public void Delete(T entity)
    {
        _store.Remove(entity.Id);
    }

    // A simple cloning method
    private T Clone(T entity)
    {
        var clone = new T();
        foreach (PropertyInfo prop in typeof(T)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanRead && p.CanWrite))
        {
            prop.SetValue(clone, prop.GetValue(entity));
        }
        return clone;
    }
}

