using CostAccounting.Services.Dtos;

namespace StockAccounting.Services.Interfaces;

public interface IStockFifoService
{
    Task<IList<StockLotDto>> GetAllStockLots();
    Task<IList<StockLotDetailsDto>> GetAllStockLotDetails();
    Task<IEnumerable<StockLotDetailsDto>> GetStockLotDetails(string ticker);

    /// <summary>
    /// Sells a specified number of shares of a stock at a given price per share.
    /// </summary>
    /// <param name="ticker">The ticker symbol of the stock to sell.</param>
    /// <param name="sharesToSell">The number of shares to sell.</param>
    /// <param name="currentPricePerShare">The current price per share.</param>
    /// <returns>A SoldStocksResultDto object containing the results of the sale.</returns>
    Task<SoldSotcksResultDto> SellStocks(string ticker, int sharesToSell, decimal currentPricePerShare);

    /// <summary>
    /// Calculates the cost basis per share (weighted average price per share)
    /// from a collection of StockLotDto records.
    /// </summary>
    /// <param name="stockLots">The stock lot DTOs.</param>
    /// <returns>The weighted average price per share.</returns>
    decimal CalculateCostBasisPerShare(IEnumerable<StockLotDto> stockLots);

    decimal CalculateProfit(int sharesToSell, decimal currentPricePerShare, decimal soldStocksCostBasis);
}


