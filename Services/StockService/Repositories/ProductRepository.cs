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

        public Task UpdateEntireProductAsync(Product product)
        {
            var affected = _stockContext.Products
                .Where(p => p.Id == product.Id)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(p => p.Name, product.Name)
                    .SetProperty(p => p.Description, product.Description)
                    .SetProperty(p => p.Price, product.Price)
                    .SetProperty(p => p.Quantity, product.Quantity)
                );
            throw new NotImplementedException();
        }
    }
}