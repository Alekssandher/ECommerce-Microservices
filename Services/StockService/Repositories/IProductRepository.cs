using StockService.Models;

namespace StockService.Repositories
{
    public interface IProductRepository
    {
        Task<Product?> GetProductByIdAsync(int productId);
        Task<List<Product>> GetAllProductsAsync();
        Task CreateProductAsync(Product product);
        Task UpdateProductAsync(Product product);
        Task ReduceProductQuantityAsync(Product product, int quantity);
    }
}