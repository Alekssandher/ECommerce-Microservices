using AuthService.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.RegisterDependencies(builder.Configuration);
builder.Services.RegisterAuthorization(builder.Configuration);

builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi().CacheOutput();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();

