using Microsoft.EntityFrameworkCore;
using StockService.Infraestructure.Data;
using StockService.Models;

namespace StockService.Repositories
{
    internal class ProductRepository : IProductRepository
    {
        private readonly StockContext _context;
        
        public ProductRepository(StockContext context)
        {
            _context = context;
        }

        public async Task CreateProductAsync(Product product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            return await _context.Products.ToListAsync();
        }

        public async Task<Product?> GetProductByIdAsync(int productId)
        {
            return await _context.Products.FirstOrDefaultAsync(p => p.Id == productId);
        }

        public async Task ReduceProductQuantityAsync(Product product, int quantity)
        {
            if (product.Quantity < quantity)
                throw new InvalidOperationException($"Insufficient product quantity. Available: {product.Quantity}, Requested: {quantity}");

            product.Quantity -= quantity;
            await _context.SaveChangesAsync();
        }

        public async Task UpdateProductAsync(Product product)
        {
            await _context.Products
                .Where(p => p.Id == product.Id)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(p => p.Name, product.Name)
                    .SetProperty(p => p.Description, product.Description)
                    .SetProperty(p => p.Price, product.Price)
                    .SetProperty(p => p.Quantity, product.Quantity)
                );
        }
    }
}