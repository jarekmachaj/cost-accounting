using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace CostAccounting.Data.Extensions;
public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        var x = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.Name.EndsWith("Service") && t.IsClass);

        foreach (var type in x)
        {
            var interfaceTypes = type.GetInterfaces()?.ToList();
            interfaceTypes?.ForEach(interfaceType =>
            {
                services.AddScoped(interfaceType, type);
            });
        }
        return services;
    }
}
