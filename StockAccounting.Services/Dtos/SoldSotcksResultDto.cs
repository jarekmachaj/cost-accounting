namespace CostAccounting.Services.Dtos;
public record SoldSotcksResultDto(StockLotDetailsDto RemainingStocks, StockLotDetailsDto SoldStocks, decimal Profit, decimal SellingPrice);
