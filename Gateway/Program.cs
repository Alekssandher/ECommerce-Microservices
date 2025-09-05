using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
builder.Configuration.AddJsonFile("routes-sales.json", optional: false, reloadOnChange: true);
builder.Configuration.AddJsonFile("routes-products.json", optional: false, reloadOnChange: true);
builder.Configuration.AddJsonFile("routes-stock.json", optional: false, reloadOnChange: true);

builder.Services.AddOcelot(builder.Configuration)
    .AddCacheManager(x =>
    {
        x.WithDictionaryHandle();
    });
    
builder.Services.AddControllers();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();
app.MapControllers();

await app.UseOcelot();

app.Run();
