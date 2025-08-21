using Microsoft.EntityFrameworkCore;
using StockService.Infraestructure.Data;

namespace StockService.Extensions
{
    internal static class DependencyInjectionExtensions
    {
        public static void RegisterDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("mysql");

            services.AddDbContext<StockContext>(
                options => options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
            );
        }
    }
}