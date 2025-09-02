using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SalesService.Infraestructure.Data;

namespace SalesService.Extensions
{
    internal static class DependencyInjectionExtensions
    {
        public static void RegisterDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("mysql");

            // Database
            services.AddDbContext<SalesContext>(
                options => options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
            );

            // Repositories
           

            // Services
            

        }
    }
}