# App description
I have updated SDK for .Net 9.0 and updated visual studio after installation. To build the app, I have used client + server template for VS. I hope it is not a problem amd will run using standard "F5" on your machine.

Solution:

![image](https://github.com/user-attachments/assets/6b856822-848b-456a-b924-a7d780ef505c)

* CostAccounting.Core - basic framework with inmemory store and simple generic repository. Solution is using in memory dictionary, which does not modify data - you can provide different inputs for same data
* CostAccounting.Data - data definition and service providers to register repositories / import data
* CostAccounting.Seervices - simple services and dtos used by controllers
* CostAccounting.client - client code (react)
* CostAccounting.Server - .Net server (ASP .Net api)


All data is harrdcoded inside `ApplyHardcodedData` method:

```csharp
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
```

There is a simple proxy mechanism to access API from client app:
![image](https://github.com/user-attachments/assets/e99cd4ff-1c08-4b74-a992-25cc6517dc66)

# If it does not work
When client code does not work, please use generated "script.ps1" to access rest API - it will give you CLI UI:

![image](https://github.com/user-attachments/assets/ca6ee9c9-5629-4e76-abfc-d13c894edd71)

# If it still does not work (maybe proxy issue)
Please contact me, or use provided unit tests that covers your scenario:
`StockServiceScenarioTests`
