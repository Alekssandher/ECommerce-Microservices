using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthService.Infraestructure.Data;
using AuthService.Repositories;
using AuthService.Services;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Extensions
{
    public static class DependencyInjectionExtensions
    {
        public static void RegisterDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("mysql");

            services.AddDbContext<UsersContext>(
                options => options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
            );

            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IUserRepository, UserRepository>();
        }
    }
}