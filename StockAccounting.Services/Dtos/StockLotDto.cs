namespace CostAccounting.Services.Dtos;
public record StockLotDto(Guid Id, string Ticker, int Shares, decimal PricePerShare, DateTime CreatedOn);

