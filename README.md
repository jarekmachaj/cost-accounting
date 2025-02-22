# App description
This solution provides a cost accounting system built with .NET 9.0 and React. The architecture follows a client-server pattern with in-memory data storage. The application demonstrates stock lot tracking with hardcoded sample data for demonstration purposes.

I have updated SDK for .Net 9.0 and updated visual studio after installation. I have used client + server template for VS - I hope it is not a problem amd will run using standard "F5" on your machine.

## System Requirements
* .NET SDK 9.0
* Visual Studio (latest version recommended)
* Node.js (for client-side dependencies)

# How to run 
```bash
git clone https://github.com/jarekmachaj/cost-accounting.git
cd cost-accounting
dotnet build
cd CostAccounting.Server
dotnet run
```
## After successful startup:
* A server console will appear
* Proxy configuration connects the React client to the ASP.NET API
  
You should see console output and new console window with client app running with proxy (go to [http://localhost:12773](https://localhost:12773/) in that case):
![image](https://github.com/user-attachments/assets/f99a8be1-473b-463b-bb07-2ecfac993879)


# Solution:

![image](https://github.com/user-attachments/assets/6b856822-848b-456a-b924-a7d780ef505c)

* CostAccounting.Core: framework foundations with In-memory data store, generic repository pattern, immutable data structure implementation
* CostAccounting.Data - data definition and service providers to register repositories / import data
* CostAccounting.Services - simple services and dtos used by controllers
* CostAccounting.client -  simple client code that pulls data with fetch
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
There is a simple proxy mechanism to access API from client app (there is a small ui bug in selling form that i did not fixed):

![image](https://github.com/user-attachments/assets/4d1e971b-e2b3-4533-8604-1aadd7531757)


# If it does not work - CLI
When client code does not work, please use generated `cost-accounting\scripts\start.ps1` to access rest API - it will give you simple CLI UI:
```powershell
# ------- part of script to set api address ------ #
# Set the base URL for the API endpoints
$apiBaseUrl = "https://localhost:7238/api/stocklots"
````

![image](https://github.com/user-attachments/assets/ca6ee9c9-5629-4e76-abfc-d13c894edd71)

# If it still does not work (maybe proxy issue)
Please contact me, or use provided unit tests that covers your scenario:
`StockServiceScenarioTests`
