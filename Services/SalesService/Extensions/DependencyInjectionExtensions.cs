using Microsoft.EntityFrameworkCore;
using SalesService.Infraestructure.Data;
using SalesService.Repositories;
using SalesService.Repositories.Interfaces;
using SalesService.Services;
using SalesService.Services.Interfaces;

namespace SalesService.Extensions
{
    internal static class DependencyInjectionExtensions
    {
        public static void RegisterDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("mysql")
                ?? throw new Exception("MySql connection string is missing.");

            // Database
            services.AddDbContext<SalesContext>(
                options => options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
            );

            // Repositories
            services.AddScoped<ISalesRepository, SalesRepository>();

            // Services
            services.AddScoped<ISalesService, Services.SalesService>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();

        }
    }
}