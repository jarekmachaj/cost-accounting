using CostAccounting.Data.Repositories.Interfaces;
using CostAccounting.Entities.Domain;
using CostAccounting.Services;
using Moq;

namespace Stocks.Tests;

public class StockServiceScenarioTests
{
    private readonly Mock<IStockRepository> _stockRepositoryMock;
    private readonly StockService _stockService;

    public StockServiceScenarioTests()
    {
        _stockRepositoryMock = new Mock<IStockRepository>();
        _stockService = new StockService(_stockRepositoryMock.Object);
    }

    [Fact]
    public async Task SellStocks_MultiplePurchases_ReturnsTwoOutputGroups_Test1()
    {
        // Scenario 1:
        //  - 100 shares purchased at $20/share in January
        //  - 150 shares purchased at $30/share in February
        //  - 120 shares purchased at $10/share in March
        // Sell 200 shares at $35/share.
        //   Sold: 100 January + 100 February
        //   Remaining: 50 February + 120 March.
        //   basis = (100*20 + 100*30) / 200 = 5000/200 = $25.

        var ticker = "TEST";
        var currentPrice = 35m;
        var stockLots = new List<StockLot>
            {
                new StockLot { Id = Guid.NewGuid(), Ticker = ticker, Shares = 100, PricePerShare = 20m, CreatedOn = new DateTime(2024, 1, 1) },
                new StockLot { Id = Guid.NewGuid(), Ticker = ticker, Shares = 150, PricePerShare = 30m, CreatedOn = new DateTime(2024, 2, 1) },
                new StockLot { Id = Guid.NewGuid(), Ticker = ticker, Shares = 120, PricePerShare = 10m, CreatedOn = new DateTime(2024, 3, 1) }
            };

        _stockRepositoryMock
            .Setup(x => x.GetFilteredStockLotsAsync(It.IsAny<Func<StockLot, bool>>()))
            .ReturnsAsync(stockLots);

        // Act:
        var result = await _stockService.SellStocks(ticker, 200, currentPrice);

        // Assert:
        Assert.Equal(2, result.SoldStocks.StockLots.Count);
        Assert.Equal(2, result.RemainingStocks.StockLots.Count);

        decimal expectedSoldCostBasis = 25m;
        Assert.Equal(expectedSoldCostBasis, Math.Round(result.SoldStocks.CostBasisPerShare, 2));

        int totalSold = result.SoldStocks.StockLots.Sum(lot => lot.Shares);
        Assert.Equal(200, totalSold);

        int totalRemaining = result.RemainingStocks.StockLots.Sum(lot => lot.Shares);
        Assert.Equal(170, totalRemaining);
    }

    [Fact]
    public async Task SellStocks_MicrosoftCostBasisCalculation_Test2()
    {
        // Scenario 2:
        //  - 100 shares of MSFT at $20/share in January
        //  - 200 shares of MSFT at $30/share in March
        // 50 shares at $40/share in April:
        // 100 shares from January + 50 shares from March.
        // basis = (100*20 + 50*30)/150 = (2000 + 1500)/150 = 3500/150 ≈ 23.33
        // profit = (40 - 23.33)*150 ≈ $2500.

        var ticker = "MSFT";
        var currentPrice = 40m;
        var stockLots = new List<StockLot>
            {
                new StockLot { Id = Guid.NewGuid(), Ticker = ticker, Shares = 100, PricePerShare = 20m, CreatedOn = new DateTime(2024, 1, 1) },
                new StockLot { Id = Guid.NewGuid(), Ticker = ticker, Shares = 200, PricePerShare = 30m, CreatedOn = new DateTime(2024, 3, 1) }
            };

        _stockRepositoryMock
            .Setup(x => x.GetFilteredStockLotsAsync(It.IsAny<Func<StockLot, bool>>()))
            .ReturnsAsync(stockLots);

        // Act
        var result = await _stockService.SellStocks(ticker, 150, currentPrice);

        // Assert:
        Assert.Equal(2, result.SoldStocks.StockLots.Count);
        Assert.Single(result.RemainingStocks.StockLots);

        decimal expectedCostBasis = 3500m / 150m;
        Assert.Equal(expectedCostBasis, result.SoldStocks.CostBasisPerShare);

        // Expected profit calculation.
        decimal expectedProfit = (currentPrice - expectedCostBasis) * 150;
        Assert.Equal(expectedProfit, result.Profit);
    }

    [Fact]
    public async Task SellStocks_MultipleLotsSpanningAll_ReturnsCorrectOutputs_Test3()
    {
        // Scenario 3:
        //  - 100 shares at $20 (January)
        //  - 150 shares at $30 (February)
        //  - 120 shares at $10 (March)
        // Sell 300 shares at $35
        // sale:
        //   Sold: 100 Jan + 150 Feb + 50 Mar = 300 shares.
        //   Remaining: 120 - 50 = 70
        //   basis = (100*20 + 150*30 + 50*10) / 300 = (2000 + 4500 + 500)/300 = 7000/300 ≈ 23.33.

        var ticker = "TEST";
        var currentPrice = 35m;
        var stockLots = new List<StockLot>
            {
                new StockLot { Id = Guid.NewGuid(), Ticker = ticker, Shares = 100, PricePerShare = 20m, CreatedOn = new DateTime(2024, 1, 1) },
                new StockLot { Id = Guid.NewGuid(), Ticker = ticker, Shares = 150, PricePerShare = 30m, CreatedOn = new DateTime(2024, 2, 1) },
                new StockLot { Id = Guid.NewGuid(), Ticker = ticker, Shares = 120, PricePerShare = 10m, CreatedOn = new DateTime(2024, 3, 1) }
            };

        _stockRepositoryMock
            .Setup(x => x.GetFilteredStockLotsAsync(It.IsAny<Func<StockLot, bool>>()))
            .ReturnsAsync(stockLots);

        // Act
        var result = await _stockService.SellStocks(ticker, 300, currentPrice);

        // Assert:
        Assert.Equal(3, result.SoldStocks.StockLots.Count);
        Assert.Single(result.RemainingStocks.StockLots);

        decimal expectedCostBasisSold = 7000m / 300m;
        Assert.Equal(expectedCostBasisSold, result.SoldStocks.CostBasisPerShare, 2);

        int totalSold = result.SoldStocks.StockLots.Sum(lot => lot.Shares);
        Assert.Equal(300, totalSold);

        int totalRemaining = result.RemainingStocks.StockLots.Sum(lot => lot.Shares);
        Assert.Equal(70, totalRemaining);

        decimal expectedProfit = (currentPrice - expectedCostBasisSold) * 300;
        Assert.Equal(expectedProfit, result.Profit);
    }
}