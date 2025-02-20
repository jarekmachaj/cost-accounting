namespace CostAccounting.Services.Dtos;

public record SaleResultDto(
    string Ticker,
    int TotalSharesSold,
    decimal TotalProfitLoss,
    IEnumerable<StockLotDetailsDto> soldLots,
    IEnumerable<StockLotDetailsDto> remainingLots
);
