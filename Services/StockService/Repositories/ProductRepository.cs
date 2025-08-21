using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StockService.Infraestructure.Data;
using StockService.Models;

namespace StockService.Repositories
{
    internal class ProductRepository : IProductRepository
    {
        private readonly StockContext _stockContext;
        public ProductRepository(StockContext stockContext)
        {
            _stockContext = stockContext;
        }

        public async Task AddStockItemAsync(Product product)
        {
            await _stockContext.Products.AddAsync(product);
            await _stockContext.SaveChangesAsync();
        }

        public async Task<List<Product>> GetAllAsync()
        {
            return await _stockContext.Products.ToListAsync();
 
        }

        public async Task<Product?> GetStockByProductIdAsync(int productId)
        {
            return await _stockContext.Products.FirstOrDefaultAsync(p => p.Id == productId);
        }

        public async Task RemoveStockAsync(Product product, int quantity)
        {
            product.Quantity -= quantity;

            await _stockContext.SaveChangesAsync();

        }

        public Task UpdateStockAsync(Product product)
        {
            throw new NotImplementedException();
        }
    }
}