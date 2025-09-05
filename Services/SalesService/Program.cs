using Shared.Extensions;
using SalesService.Extensions;
using Shared.Middlewares;
using Shared.ModelViews;
using SalesService.Consumers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.RegisterDependencies(builder.Configuration);
builder.Services.RegisterAuthorization(builder.Configuration);
builder.Services.AddRabbit(builder.Configuration, bus =>
{
    bus.AddConsumer<SaleCreationFailedConsumer>();
    bus.AddConsumer<SaleItemsReserved>();
}
);

builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi()
        .CacheOutput();
    app.MapHealthChecks("/health");
}

app.UseHttpsRedirection();

app.UseMiddleware<GlobalExceptionHandler>();

app.MapControllers();

app.Run();

