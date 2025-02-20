# App description
I have updated SDK for .Net 9.0 and updated visual studio after installation. To build the app, I have used client + server template for VS. I hope it is not a problem amd will run using standard "F5" on your machine.

# How to run 
```bash
git clone https://github.com/jarekmachaj/cost-accounting.git
cd cost-accounting
dotnet build
cd CostAccounting.Server
dotnet run
```
you should see console output and new console window with client app running with proxy (go to [http://localhost:12773](https://localhost:12773/) in that case):

![image](https://github.com/user-attachments/assets/f99a8be1-473b-463b-bb07-2ecfac993879)


# Solution:

![image](https://github.com/user-attachments/assets/6b856822-848b-456a-b924-a7d780ef505c)

* CostAccounting.Core - basic framework with inmemory store and simple generic repository. Solution is using in memory dictionary, which does not modify data - you can provide different inputs for same data
* CostAccounting.Data - data definition and service providers to register repositories / import data
* CostAccounting.Seervices - simple services and dtos used by controllers
* CostAccounting.client - client code (react)
* CostAccounting.Server - .Net server (ASP .Net api)
* Stocks.Tests - xUnit test project


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

# Web UI
There is a simple proxy mechanism to access API from client app:

![image](https://github.com/user-attachments/assets/4d1e971b-e2b3-4533-8604-1aadd7531757)


# If it does not work - CLI
When client code does not work, please use generated "script.ps1" to access rest API - it will give you CLI UI:

![image](https://github.com/user-attachments/assets/ca6ee9c9-5629-4e76-abfc-d13c894edd71)

# If it still does not work (maybe proxy issue)
Please contact me, or use provided unit tests that covers your scenario:
`StockServiceScenarioTests`
