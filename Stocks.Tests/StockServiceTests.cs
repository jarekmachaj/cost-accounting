using CostAccounting.Data.Repositories.Interfaces;
using CostAccounting.Entities.Domain;
using CostAccounting.Services;
using Moq;

namespace Stocks.Tests;

public class StockServiceTests
{
    private readonly Mock<IStockRepository> _stockRepositoryMock;
    private readonly StockService _stockService;

    public StockServiceTests()
    {
        _stockRepositoryMock = new Mock<IStockRepository>();
        _stockService = new StockService(_stockRepositoryMock.Object);
    }

    [Fact]
    public async Task SellStocks_WhenSellingAllShares_ReturnsCorrectResult()
    {
        // Arrange
        var ticker = "AAPL";
        var currentPrice = 200m;
        var stockLots = new List<StockLot>
        {
            new() { Id = Guid.NewGuid(), Ticker = ticker, Shares = 10, PricePerShare = 100m, CreatedOn = DateTime.Now.AddDays(-2) },
            new() { Id = Guid.NewGuid(), Ticker = ticker, Shares = 5, PricePerShare = 150m, CreatedOn = DateTime.Now }
        };

        _stockRepositoryMock.Setup(x => x.GetFilteredAsync(It.IsAny<Func<StockLot, bool>>())).ReturnsAsync(stockLots);

        // Act
        var result = await _stockService.SellStocks(ticker, 15, currentPrice);
        var profit = _stockService.CalculateProfit(15, currentPrice, result.SoldStocks.CostBasisPerShare);

        // Assert
        Assert.Equal(profit, result.Profit); // Revenue based on provided current price
        Assert.Empty(result.RemainingStocks.StockLots);
        Assert.Equal(2, result.SoldStocks.StockLots.Count);
        Assert.Equal(116.67m, Math.Round(result.SoldStocks.CostBasisPerShare, 2)); // (10*100 + 5*150)/15
    }

    [Fact]
    public async Task SellStocks_WhenSellingPartialShares_ReturnsCorrectResult()
    {
        // Arrange
        var ticker = "AAPL";
        var currentPrice = 180m;
        var stockLots = new List<StockLot>
        {
            new() { Id = Guid.NewGuid(), Ticker = ticker, Shares = 10, PricePerShare = 100m, CreatedOn = DateTime.Now.AddDays(-2) },
            new() { Id = Guid.NewGuid(), Ticker = ticker, Shares = 5, PricePerShare = 150m, CreatedOn = DateTime.Now }
        };

        _stockRepositoryMock.Setup(x => x.GetFilteredAsync(It.IsAny<Func<StockLot, bool>>()))
            .ReturnsAsync(stockLots);

        // Act
        var result = await _stockService.SellStocks(ticker, 8, currentPrice);
        var profit = _stockService.CalculateProfit(8, currentPrice, result.SoldStocks.CostBasisPerShare);

        Assert.Equal(profit, result.Profit);
        Assert.Single(result.RemainingStocks.StockLots, x => x.Shares == 2);
        Assert.Equal(5, result.RemainingStocks.StockLots.Last().Shares);
        Assert.Equal(100m, result.SoldStocks.CostBasisPerShare);
    }

    [Fact]
    public Task SellStocks_WhenInsufficientShares_ThrowsException()
    {
        // Arrange
        var ticker = "AAPL";
        var currentPrice = 120m;
        var stockLots = new List<StockLot>
        {
            new() { Id = Guid.NewGuid(), Ticker = ticker, Shares = 10, PricePerShare = 100m, CreatedOn = DateTime.Now }
        };

        _stockRepositoryMock.Setup(x => x.GetFilteredAsync(It.IsAny<Func<StockLot, bool>>()))
            .ReturnsAsync(stockLots);

        // Act & Assert
        return Assert.ThrowsAsync<InvalidOperationException>(
            () => _stockService.SellStocks(ticker, 15, currentPrice)
        );
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public Task SellStocks_WhenInvalidShareCount_ThrowsException(int sharesToSell)
    {
        // Act & Assert
        return Assert.ThrowsAsync<ArgumentException>(
            () => _stockService.SellStocks("AAPL", sharesToSell, 100m)
        );
    }
} 