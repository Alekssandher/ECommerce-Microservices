using Microsoft.EntityFrameworkCore;
using SalesService.Models;

namespace SalesService.Infraestructure.Data
{
    internal class SalesContext : DbContext
    {
        private readonly IConfiguration _config;

        public SalesContext(IConfiguration config)
        {
            _config = config;
        }

        internal DbSet<Sale> Sales { get; set; }

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