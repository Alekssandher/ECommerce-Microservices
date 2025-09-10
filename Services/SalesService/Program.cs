using Shared.Extensions;
using SalesService.Extensions;
using Shared.Middlewares;
using SalesService.Consumers;
using Serilog;
using SalesService.Infraestructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.RegisterDependencies(builder.Configuration);
builder.Services.RegisterAuthorization(builder.Configuration);

builder.Services.RegisterSeriLog("sales-service");

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

app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi()
        .CacheOutput();

}
var isDocker = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";

if (isDocker)
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<SalesContext>();
    await db.Database.MigrateAsync();
}
app.MapHealthChecks("/health");
app.UseHttpsRedirection();

app.UseMiddleware<GlobalExceptionHandler>();

app.MapControllers();

app.Run();

