using Microsoft.EntityFrameworkCore;
using Shared.Exceptions;
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

        public async Task CreateProductAsync(Product product, int initialStock)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            
            try
            {
                await _context.Products.AddAsync(product);
                await _context.SaveChangesAsync();

                var stockItem = new StockItem
                {
                    ProductId = product.Id,
                    QuantityAvailable = initialStock,
                    QuantityReserved = 0
                };

                await _context.Stocks.AddAsync(stockItem);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            return await _context.Products.ToListAsync();
        }

        public async Task GetProductsByIdsAsync(List<int> productIds)
        {
            var stockItems = await _context.Stocks
                                   .Where(s => productIds.Contains(s.ProductId))
                                   .ToListAsync();

            var missingIds = productIds.Except(stockItems.Select(s => s.ProductId)).ToList();

            if (missingIds.Count <= 0)
                throw new Exceptions.NotFoundException($"Products not found: {string.Join(", ", missingIds)}");

        }

        public async Task<Product?> GetProductByIdAsync(int productId)
        {
            return await _context.Products.FirstOrDefaultAsync(p => p.Id == productId);
        }

        public async Task UpdateProductAsync(Product product)
        {
            await _context.Products
                .Where(p => p.Id == product.Id)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(p => p.Name, product.Name)
                    .SetProperty(p => p.Description, product.Description)
                    .SetProperty(p => p.Price, product.Price)
                );
        }

        public async Task<decimal> GetProductPriceByIdAsync(int productId)
        {
            var price = await _context.Products
                              .Where(p => p.Id == productId)
                              .Select(p => p.Price)
                              .FirstOrDefaultAsync();

            return price;
        }
    }
}