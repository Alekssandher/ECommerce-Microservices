using MassTransit;
using Microsoft.EntityFrameworkCore;
using StockService.Infraestructure.Data;
using StockService.Repositories;
using StockService.Services;

namespace StockService.Extensions
{
    internal static class DependencyInjectionExtensions
    {
        public static void RegisterDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("mysql");

            // Database
            services.AddDbContext<StockContext>(
                options => options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
            );

            // Repositories
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IStockRepository, StockRepository>();

            // Services
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IStockManagementService, StockManagementService>();

        }
    }
}