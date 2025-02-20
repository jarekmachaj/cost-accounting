using CostAccounting.Core.Entities;

namespace CostAccounting.Data.Entities.Base
{
    public class CostAccountingEntityBase : IEntity<Guid>
    {
        public Guid Id { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
