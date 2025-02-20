using CostAccounting.Core.Data.Interfaces;
using CostAccounting.Entities.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace CostAccounting.Data.Extensions
{
    public static class IServiceProviderExtensions
    {
        public static IServiceProvider ApplyHardcodedData(this IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var store = scope.ServiceProvider.GetRequiredService<IInMemoryDataStore<StockLot, Guid>>();
            store.Add(new StockLot(Guid.NewGuid(), "Microsoft", 100, 20, new DateTime(2024, 01, 1)));
            store.Add(new StockLot(Guid.NewGuid(), "Microsoft", 150, 30, new DateTime(2024, 02, 1)));
            store.Add(new StockLot(Guid.NewGuid(), "Microsoft", 120, 10, new DateTime(2024, 03, 1)));
            return serviceProvider;
        }
    }
}
