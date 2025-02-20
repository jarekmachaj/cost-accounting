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
            store.Add(new StockLot(Guid.NewGuid(), "Microsoft", 100, 20, new DateTime(2024, 04, 1)));
            store.Add(new StockLot(Guid.NewGuid(), "Microsoft", 200, 30, new DateTime(2024, 04, 2)));
            return serviceProvider;
        }
    }
}
