using Microsoft.EntityFrameworkCore;
using Shared.Extensions;
using Shared.Middlewares;
using StockService.Consumers;
using StockService.Extensions;
using StockService.Infraestructure.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.RegisterDependencies(builder.Configuration);
builder.Services.AddRabbit(builder.Configuration, bus =>
{
    bus.AddConsumer<SaleCanceledConsumer>();
    bus.AddConsumer<SaleConfirmedConsumer>();
    bus.AddConsumer<SaleCreatedConsumer>();
});

builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

var isDocker = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Docker";

if (isDocker)
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<StockContext>();
    await db.Database.MigrateAsync();
}

app.MapHealthChecks("/health");

app.UseHttpsRedirection();

app.UseMiddleware<GlobalExceptionHandler>();

app.MapControllers();

app.Run();

