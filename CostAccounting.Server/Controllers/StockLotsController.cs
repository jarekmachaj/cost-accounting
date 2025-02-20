using CostAccounting.Services.Dtos;
using Microsoft.AspNetCore.Mvc;
using StockAccounting.Services.Interfaces;

namespace CostAccounting.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StockLotsController : ControllerBase
{
    private readonly IStockFifoService stockService;

    public StockLotsController(IStockFifoService stockService)
    {
        this.stockService = stockService;
    }

    [HttpGet]
    public Task<IList<StockLotDetailsDto>> Get()
    {
        return stockService.GetAllStockLotDetails();
    }

    [HttpPost("{ticker}/sell")]
    public Task<SoldSotcksResultDto> Sell(string ticker, [FromBody] SaleRequestDto request)
    {
        var result = stockService.SellStocks(ticker, request.SharesToSell, request.SellingPrice);
        return result;
    }
}
