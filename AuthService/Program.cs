using AuthService.Extensions;
using Shared.Extensions;
using Shared.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.RegisterDependencies(builder.Configuration);
builder.Services.RegisterAuthorization(builder.Configuration);
builder.Services.RegisterSeriLog("auth-service");

builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi().CacheOutput();
}

app.MapHealthChecks("/health");

app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();
app.MapControllers();

app.UseMiddleware<GlobalExceptionHandler>();

app.Run();

