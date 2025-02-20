using CostAccounting.Services;

namespace Stocks.Tests;

public class CalculateProfitTests
{
    private readonly StockService _stockService = new StockService(null);

    [Fact]
    public void CalculateProfit_PositiveProfit_ReturnsCorrectValue()
    {
        // Arrange
        int sharesToSell = 10;
        decimal currentPricePerShare = 50m;
        decimal soldStocksCostBasis = 40m;
        // (50 - 40) * 10 = 10 * 10 = 100
        decimal expectedProfit = 100m;

        // Act
        decimal profit = _stockService.CalculateProfit(sharesToSell, currentPricePerShare, soldStocksCostBasis);

        // Assert
        Assert.Equal(expectedProfit, profit);
    }

    [Fact]
    public void CalculateProfit_NegativeProfit_ReturnsCorrectValue()
    {
        // Arrange
        int sharesToSell = 5;
        decimal currentPricePerShare = 20m;
        decimal soldStocksCostBasis = 25m;
        // (20 - 25) * 5 = (-5) * 5 = -25
        decimal expectedProfit = -25m;

        // Act
        decimal profit = _stockService.CalculateProfit(sharesToSell, currentPricePerShare, soldStocksCostBasis);

        // Assert
        Assert.Equal(expectedProfit, profit);
    }

    [Fact]
    public void CalculateProfit_ZeroShares_ReturnsZeroProfit()
    {
        // Arrange
        int sharesToSell = 0;
        decimal currentPricePerShare = 100m;
        decimal soldStocksCostBasis = 50m;
        // (100 - 50) * 0 = 0
        decimal expectedProfit = 0m;

        // Act
        decimal profit = _stockService.CalculateProfit(sharesToSell, currentPricePerShare, soldStocksCostBasis);

        // Assert
        Assert.Equal(expectedProfit, profit);
    }
}