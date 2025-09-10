using Shared.Extensions;
using Shared.Middlewares;
using StockService.Consumers;
using StockService.Extensions;

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
app.MapHealthChecks("/health");

app.UseHttpsRedirection();

app.UseMiddleware<GlobalExceptionHandler>();

app.MapControllers();

app.Run();

