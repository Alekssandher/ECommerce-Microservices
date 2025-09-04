using StockService.Models;

namespace StockService.Repositories
{
    public interface IProductRepository
    {
        Task<Product?> GetProductByIdAsync(int productId);
        Task GetProductsByIdsAsync(List<int> productIds);
        Task<List<Product>> GetAllProductsAsync();
        Task CreateProductAsync(Product product, int initialStock);
        Task UpdateProductAsync(Product product);
    }
}