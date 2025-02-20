namespace CostAccounting.Services.Dtos;
public record StockLotDetailsDto(List<StockLotDto> StockLots, decimal CostBasisPerShare);
