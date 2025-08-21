using StockService.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.RegisterDependencies(builder.Configuration);

builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();

