using Autofac.Extensions.DependencyInjection;
using CostAccounting.Core.Data;
using CostAccounting.Core.Data.Interfaces;
using CostAccounting.Data.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Services.AddSingleton(typeof(IInMemoryDataStore<,>), typeof(InMemoryDataStore<,>));
builder.Services.AddRepositories();
builder.Services.AddServices();
builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();
app.Services.ApplyHardcodedData();
app.UseDefaultFiles();
app.MapStaticAssets();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapControllers();
app.MapFallbackToFile("/index.html");

app.Run();
