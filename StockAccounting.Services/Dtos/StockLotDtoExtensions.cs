using CostAccounting.Entities.Domain;

namespace CostAccounting.Services.Dtos;
public static class StockLotDtoExtensions
{
    public static StockLotDto ToDto(this StockLot stockLot)
    {
        return new StockLotDto(stockLot.Id, stockLot.Ticker, stockLot.Shares, stockLot.PricePerShare, stockLot.CreatedOn);
    }
    public static StockLot ToEntity(this StockLotDto stockLotDto)
    {
        return new StockLot(stockLotDto.Id, stockLotDto.Ticker, stockLotDto.Shares, stockLotDto.PricePerShare);
    }
}
