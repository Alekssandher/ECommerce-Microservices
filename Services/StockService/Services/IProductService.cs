using StockService.Dtos;

namespace StockService.Services
{
    public interface IProductService
    {
        Task CreateProductAsync(CreateProductRequest request);
        Task UpdateProductAsync(UpdateProductRequest request);
        Task RemoveProductQuantityAsync(int productId, int quantity);
        Task<ProductResponse> GetProductByIdAsync(int productId);
        Task<List<ProductResponse>> GetAllProductsAsync();
    }
}