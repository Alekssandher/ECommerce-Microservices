using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StockService.Models;

namespace StockService.Repositories
{
    public interface IProductRepository
    {
        Task<Product?> GetStockByProductIdAsync(int productId);
        Task<List<Product>> GetAllAsync();
        Task AddStockItemAsync(Product product);
        Task UpdateEntireProductAsync(Product product);
        Task RemoveStockAsync(Product product, int quantity);

    }
}