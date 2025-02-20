using CostAccounting.Data.Repositories.Interfaces;
using CostAccounting.Services.Dtos;
using StockAccounting.Services.Interfaces;

namespace CostAccounting.Services;

public class StockService(IStockRepository stockRepository) : IStockService
{
    public async Task<IList<StockLotDetailsDto>> GetAllStockLotDetails()
    {
        var lots = await stockRepository.GetAllAsync();

        // Group the lots by ticker
        var groupedLots = lots.GroupBy(lot => lot.Ticker);

        var detailsList = new List<StockLotDetailsDto>();

        foreach (var group in groupedLots)
        {
            // Convert each StockLot to a StockLotDto
            var stockLotDtos = group
                .Select(lot => lot.ToDto())
                .ToList();

            // Calculate the cost basis per share using the helper method
            decimal costBasisPerShare = CalculateCostBasisPerShare(stockLotDtos);

            // Create the details record for this group
            detailsList.Add(new StockLotDetailsDto(stockLotDtos, costBasisPerShare));
        }

        return detailsList;
    }

    public async Task<IList<StockLotDto>> GetAllStockLots()
    {
        var lots = await stockRepository.GetAllStockLots();
        return lots.Select(x => x.ToDto()).ToList();
    }

    public async Task<IEnumerable<StockLotDetailsDto>> GetStockLotDetails(string ticker)
    {
        var lots = await stockRepository.GetFilteredAsync(lot =>
            lot.Ticker.Equals(ticker, StringComparison.InvariantCultureIgnoreCase));

        // Group the lots by ticker
        var groupedLots = lots.GroupBy(lot => lot.Ticker);

        var detailsList = new List<StockLotDetailsDto>();

        foreach (var group in groupedLots)
        {
            // Convert each StockLot to a StockLotDto
            var stockLotDtos = group
                .Select(lot => lot.ToDto())
                .ToList();

            // Calculate the cost basis per share using the helper method
            decimal costBasisPerShare = CalculateCostBasisPerShare(stockLotDtos);

            // Create the details record for this group
            detailsList.Add(new StockLotDetailsDto(stockLotDtos, costBasisPerShare));
        }

        return detailsList;
    }
    public decimal CalculateCostBasisPerShare(IEnumerable<StockLotDto> stockLots)
    {
        // Sum up the total number of shares
        int totalShares = stockLots.Sum(lot => lot.Shares);
        if (totalShares == 0)
        {
            return 0;
        }

        decimal totalCost = stockLots.Sum(lot => lot.PricePerShare * lot.Shares);

        return totalCost / totalShares;
    }

    public async Task<SoldSotcksResultDto> SellStocks(string ticker, int sharesToSell, decimal currentPricePerShare)
    {
        if (sharesToSell <= 0)
            throw new ArgumentException("Number of shares to sell must be positive", nameof(sharesToSell));

        // Retrieve all lots for the given ticker, sorted by purchase date (FIFO)
        var lots = await stockRepository.GetFilteredAsync(lot =>
            lot.Ticker.Equals(ticker, StringComparison.InvariantCultureIgnoreCase));

        var orderedLots = lots.OrderBy(x => x.CreatedOn).ToList();

        int totalAvailableShares = orderedLots.Sum(lot => lot.Shares);
        if (totalAvailableShares < sharesToSell)
            throw new InvalidOperationException($"Insufficient shares available. Requested: {sharesToSell}, Available: {totalAvailableShares}");

        var soldLots = new List<StockLotDto>();
        var remainingLots = new List<StockLotDto>();
        int sharesStillToSell = sharesToSell;

        foreach (var lot in orderedLots)
        {
            if (sharesStillToSell > 0)
            {
                if (lot.Shares <= sharesStillToSell)
                {
                    soldLots.Add(lot.ToDto());
                    sharesStillToSell -= lot.Shares;
                }
                else
                {
                    // Split the lot
                    var soldPortion = lot.ToDto() with { Shares = sharesStillToSell };
                    var remainingPortion = lot.ToDto() with { Shares = lot.Shares - sharesStillToSell };

                    soldLots.Add(soldPortion);
                    remainingLots.Add(remainingPortion);
                    sharesStillToSell = 0;
                }
            }
            else
            {
                remainingLots.Add(lot.ToDto());
            }
        }

        // Calculate the cost basis per share for the sold lots
        var soldStocksCostBasis = CalculateCostBasisPerShare(soldLots);

        // Calculate the profit:
        // (selling price per share - cost basis per share) * number of shares sold
        decimal profit = CalculateProfit(sharesToSell, currentPricePerShare, soldStocksCostBasis);

        return new SoldSotcksResultDto(
            new StockLotDetailsDto(remainingLots, CalculateCostBasisPerShare(remainingLots)),
            new StockLotDetailsDto(soldLots, soldStocksCostBasis),
            profit,
            currentPricePerShare
        );
    }
    public decimal CalculateProfit(int sharesToSell, decimal currentPricePerShare, decimal soldStocksCostBasis)
    {
        return (currentPricePerShare - soldStocksCostBasis) * sharesToSell;
    }
}