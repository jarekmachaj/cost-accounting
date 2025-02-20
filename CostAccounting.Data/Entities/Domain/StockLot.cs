using CostAccounting.Data.Entities.Base;
namespace CostAccounting.Entities.Domain;

public class StockLot : CostAccountingEntityBase
{
    public StockLot()
    {
    }

    public StockLot(Guid id, string ticker, int shares, decimal pricePerShare, DateTime? createdOn = null)
    {
        Id = id;
        Shares = shares;
        PricePerShare = pricePerShare;
        Ticker = ticker;
        CreatedOn = createdOn != null ? createdOn.GetValueOrDefault() : DateTime.UtcNow;
    }

    public int Shares { get; set; }

    public string Ticker { get; set; }

    public decimal PricePerShare { get; set; }
}
