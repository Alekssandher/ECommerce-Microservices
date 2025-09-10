using AuthService.Extensions;
using AuthService.Infraestructure.Data;
using Microsoft.EntityFrameworkCore;
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

var isDocker = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Docker";

if (isDocker)
{
    Console.WriteLine(isDocker);
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<UsersContext>();
    await db.Database.MigrateAsync();
    await SeedData.EnsureTestUserAsync(db);
    
}


app.MapHealthChecks("/health");

app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();
app.MapControllers();

app.UseMiddleware<GlobalExceptionHandler>();

app.Run();

