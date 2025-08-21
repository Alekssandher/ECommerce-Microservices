using StockService.Dtos;
using StockService.Mappers;
using StockService.Repositories;
using Shared.Exceptions;

namespace StockService.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task CreateProduct(ProductRequest productRequest)
        {
            await _productRepository.AddStockItemAsync(productRequest.ToModel());
        }

        public async Task DeleteProduct(int productId, int quantity)
        {
            var product = await _productRepository.GetStockByProductIdAsync(productId) ?? throw new Exceptions.NotFoundException("Product Not Found");

            await _productRepository.RemoveStockAsync(product, quantity);
        }

        public async Task<List<ProductResponse>> GetAllProducts()
        {
            var res =  await _productRepository.GetAllAsync();

            return res.ToProductList();
        }

        public async Task<ProductResponse> GetProductById(int productId)
        {
            var res = await _productRepository.GetStockByProductIdAsync(productId) ?? throw new Exceptions.NotFoundException("Product Not Found");

            return res.ToResponse();
        }

        public async Task UpdateProduct(ProductUpdateRequest productRequest)
        {
            if (productRequest.Id == 0) throw new Exceptions.BadRequestException("Product Id cannot be zero or Null.");

            var product = await _productRepository.GetStockByProductIdAsync(productRequest.Id) ?? throw new Exceptions.NotFoundException("Could Not Find a Product With this ID");

            if (productRequest.Name != null) product.Name = productRequest.Name;
            if (productRequest.Description != null) product.Description = productRequest.Description;
            if (productRequest.Price != null) product.Price = (decimal)productRequest.Price;
            if (productRequest.Quantity != null) product.Quantity = (int)productRequest.Quantity;

            await _productRepository.UpdateEntireProductAsync(product);
        }
    }
}