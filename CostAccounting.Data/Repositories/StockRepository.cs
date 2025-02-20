using CostAccounting.Core.Data;
using CostAccounting.Core.Data.Interfaces;
using CostAccounting.Data.Repositories.Interfaces;
using CostAccounting.Entities.Domain;

namespace CostAccounting.Data.Repositories;

public class StockRepository : GenericRepository<StockLot, Guid>, IStockRepository
{
    public StockRepository(IInMemoryDataStore<StockLot, Guid> store): base(store) { }

    public async Task<IEnumerable<StockLot>> GetAllStockLots()
    {
        var stockLots = await GetAllAsync();
        return stockLots.OrderBy(l => l.Id).OrderBy(x => x.CreatedOn);
    }

    public async Task<IEnumerable<StockLot>> GetStockLotByTicker(string ticker)
    {
        var stockLots = await GetFilteredAsync(x => x.Ticker.Equals(ticker, StringComparison.InvariantCultureIgnoreCase));
        return stockLots.OrderBy(x => x.CreatedOn);
    }

    public async Task<IEnumerable<StockLot>> GetFilteredStockLotsAsync(Func<StockLot, bool> predicate)
    {
        var filteredEntities = await GetFilteredAsync(predicate);
        return filteredEntities.OrderBy(x => x.CreatedOn);
    }
}
