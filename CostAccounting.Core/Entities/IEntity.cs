namespace CostAccounting.Core.Entities;

public interface IEntity<TKey>
{
    TKey Id { get; set; }
    DateTime CreatedOn { get; set; }
}

public interface IEntity : IEntity<int> { }
