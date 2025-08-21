using Microsoft.EntityFrameworkCore;
using StockService.Models;

namespace StockService.Infraestructure.Data
{
    internal class StockContext : DbContext
    {
        private readonly IConfiguration _config;

        public StockContext(IConfiguration config)
        {
            _config = config;
        }

        public DbSet<Product> Products { get; set; } = default!;
        public DbSet<StockItem> Stocks { get; set; } = default!;
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