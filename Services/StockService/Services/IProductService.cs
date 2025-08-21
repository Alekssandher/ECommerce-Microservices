using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StockService.Dtos;
using StockService.Models;

namespace StockService.Services
{
    public interface IProductService
    {
        Task CreateProduct(ProductRequest productRequest);
        Task UpdateProduct(ProductUpdateRequest productRequest);
        Task DeleteProduct(int productId, int quantity);
        Task<ProductResponse> GetProductById(int productId);
        Task<List<ProductResponse>> GetAllProducts();
    }
}