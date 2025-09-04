using StockService.Dtos;

namespace StockService.Services
{
    public interface IProductService
    {
        Task CreateProductAsync(CreateProductRequest request);
        Task<decimal> GetProductPriceByIdAsync(int productId);
        Task UpdateProductAsync(UpdateProductRequest request);
        Task<ProductResponse> GetProductByIdAsync(int productId);
        Task<List<ProductResponse>> GetAllProductsAsync();
    }
}