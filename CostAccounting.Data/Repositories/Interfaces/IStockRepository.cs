using CostAccounting.Core.Data.Interfaces;
using CostAccounting.Entities.Domain;

namespace CostAccounting.Data.Repositories.Interfaces;
public interface IStockRepository : IGenericRepository<StockLot, Guid>
{
    Task<IEnumerable<StockLot>> GetAllStockLots();
    Task<IEnumerable<StockLot>> GetStockLotByTicker(string ticker);
    Task<IEnumerable<StockLot>> GetFilteredStockLotsAsync(Func<StockLot, bool> predicate);
}