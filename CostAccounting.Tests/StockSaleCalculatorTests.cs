using CostAccounting.Entities.Domain;
using CostAccounting.Services;
using CostAccounting.Services.Dtos;

namespace CostAccounting.Tests;

public class StockSaleCalculatorTests
{
    [Fact]
    public void CalculateSale_Sell150SharesAt40_ShouldReturnExpectedResults()
    {
        // Arrange
        // Purchase lots: 100 shares @ $20, 150 shares @ $30, 120 shares @ $10
        var lots = new List<StockLot>
            {
                new StockLot(1, 100, 30m),
                new StockLot(2, 150, 30),
                new StockLot(3, 120, 10)
            };

        var calculator = new StockSaleCalculatorService();
        int sharesToSell = 150;
        decimal sellingPrice = 40m;

        // Act
        // Expect to sell 100 shares from the first lot and 50 shares from the second.
        var result = calculator.CalculateSale(sharesToSell, sellingPrice, lots.Select(x => x.ToDto()).ToList());

        // Assert
        // Total sold shares should be 150.
        Assert.Equal(150, result.TotalSharesSold);
        // Total cost basis: 100 * 20 + 50 * 30 = 2000 + 1500 = 3500.
        Assert.Equal(3500m, result.TotalCostBasis);
        // Total revenue: 150 * 40 = 6000.
        Assert.Equal(6000m, result.TotalRevenue);
        // Profit/loss: 6000 - 3500 = 2500.
        Assert.Equal(2500m, result.TotalProfitLoss);

        // Check sold details:
        Assert.Equal(2, result.SoldDetails.Count);
        Assert.Contains(result.SoldDetails, s => s.SharesSold == 100 && s.PurchasePrice == 20m);
        Assert.Contains(result.SoldDetails, s => s.SharesSold == 50 && s.PurchasePrice == 30m);

        // Remaining lots should be:
        // Lot2: remaining 150 - 50 = 100 shares @ $30, Lot3: 120 shares @ $10.
        Assert.Equal(2, result.RemainingLots.Count);
        Assert.Contains(result.RemainingLots, r => r.Shares == 100 && r.PurchasePrice == 30m);
        Assert.Contains(result.RemainingLots, r => r.Shares == 120 && r.PurchasePrice == 10m);

        // Total remaining shares = 100 + 120 = 220.
        int totalRemaining = result.RemainingLots.Sum(r => r.Shares);
        Assert.Equal(220, totalRemaining);
    }

    [Fact]
    public void CalculateSale_Sell400SharesAt40_ShouldSellAllAvailableShares()
    {
        // Arrange
        // Total available shares = 100 + 150 + 120 = 370.
        var lots = new List<StockLot>
            {
                new StockLot(1, 100, 30m),
                new StockLot(2, 150, 30),
                new StockLot(3, 120, 10)
            };

        var calculator = new StockSaleCalculatorService();
        int sharesToSell = 400; // Request exceeds available shares.
        decimal sellingPrice = 40m;

        // Act
        // Expect to sell all available shares (370) with no remaining lots.
        var result = calculator.CalculateSale(sharesToSell, sellingPrice, lots.Select(x => x.ToDto()).ToList());

        // Assert
        int totalAvailable = 100 + 150 + 120;
        Assert.Equal(totalAvailable, result.TotalSharesSold);
        // Total cost basis = 100*20 + 150*30 + 120*10 = 2000 + 4500 + 1200 = 7700.
        Assert.Equal(7700m, result.TotalCostBasis);
        // Total revenue = 370 * 40 = 14800.
        Assert.Equal(14800m, result.TotalRevenue);
        Assert.Equal(14800m - 7700m, result.TotalProfitLoss);

        // Verify sold details for each lot.
        Assert.Equal(3, result.SoldDetails.Count);
        Assert.Contains(result.SoldDetails, s => s.SharesSold == 100 && s.PurchasePrice == 20m);
        Assert.Contains(result.SoldDetails, s => s.SharesSold == 150 && s.PurchasePrice == 30m);
        Assert.Contains(result.SoldDetails, s => s.SharesSold == 120 && s.PurchasePrice == 10m);

        // No remaining lots.
        Assert.Empty(result.RemainingLots);
    }

    [Fact]
    public void CalculateSale_SellZeroShares_ShouldReturnNoSale()
    {
        // Arrange
        var lots = new List<StockLot>
           {
                new StockLot(1, 100, 30m),
                new StockLot(2, 150, 30),
                new StockLot(3, 120, 10)
            };

        var calculator = new StockSaleCalculatorService();
        int sharesToSell = 0;
        decimal sellingPrice = 40m;

        // Act
        var result = calculator.CalculateSale(sharesToSell, sellingPrice, lots.Select(x => x.ToDto()).ToList());

        // Assert
        Assert.Equal(0, result.TotalSharesSold);
        Assert.Equal(0m, result.TotalCostBasis);
        Assert.Equal(0m, result.TotalRevenue);
        Assert.Equal(0m, result.TotalProfitLoss);
        Assert.Empty(result.SoldDetails);

        // All original lots remain intact.
        Assert.Equal(3, result.RemainingLots.Count);
        int totalRemaining = result.RemainingLots.Sum(r => r.Shares);
        Assert.Equal(370, totalRemaining);
    }

    [Fact]
    public void CalculateSale_SellAllShares_ShouldLeaveNoRemainingLots()
    {
        // Arrange
        var lots = new List<StockLot>
            {
                new StockLot(1, 100, 30m),
                new StockLot(2, 150, 30),
                new StockLot(3, 120, 10)
            };

        var calculator = new StockSaleCalculatorService();
        int sharesToSell = 370; // Exactly the total available shares.
        decimal sellingPrice = 40m;

        // Act
        var result = calculator.CalculateSale(sharesToSell, sellingPrice, lots.Select(x => x.ToDto()).ToList());

        // Assert
        Assert.Equal(370, result.TotalSharesSold);
        Assert.Equal(7700m, result.TotalCostBasis);
        Assert.Equal(370 * 40m, result.TotalRevenue);
        Assert.Equal(370 * 40m - 7700m, result.TotalProfitLoss);
        // There should be no remaining lots.
        Assert.Empty(result.RemainingLots);
    }
}