using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

namespace SalesService.Infraestructure.Data
{
    internal class SalesContext : DbContext
    {
        private readonly IConfiguration _config;

        public SalesContext(IConfiguration config)
        {
            _config = config;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = _config.GetConnectionString("mysql") ?? throw new Exception("Connection String Is Missing.");

            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySql(
                    connectionString,
                    ServerVersion.AutoDetect(connectionString)
                );
            }
        }
    }
}