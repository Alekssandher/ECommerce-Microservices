using StockService.Dtos;
using StockService.Mappers;
using StockService.Repositories;
using Shared.Exceptions;

namespace StockService.Services
{
    internal class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        
        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task CreateProductAsync(CreateProductRequest request)
        {
            var product = request.ToProductModel();
            await _productRepository.CreateProductAsync(product, request.InitialQuantity);
        }

        public async Task<List<ProductResponse>> GetAllProductsAsync()
        {
            var products = await _productRepository.GetAllProductsAsync();
            return products.ToProductResponseList();
        }

        public async Task<ProductResponse> GetProductByIdAsync(int productId)
        {
            var product = await _productRepository.GetProductByIdAsync(productId) 
                ?? throw new Exceptions.NotFoundException("Product not found");

            return product.ToProductResponse();
        }

        public async Task<decimal> GetProductPriceByIdAsync(int productId)
        {
            return await _productRepository.GetProductPriceByIdAsync(productId);
        }
        public async Task UpdateProductAsync(UpdateProductRequest request)
        {
            if (request.ProductId <= 0)
                throw new Exceptions.BadRequestException("Product ID must be greater than zero");

            var product = await _productRepository.GetProductByIdAsync(request.ProductId)
                ?? throw new Exceptions.NotFoundException("Product not found");

            if (request.Name != null) product.Name = request.Name;
            if (request.Description != null) product.Description = request.Description;
            if (request.Price.HasValue) product.Price = request.Price.Value;

            await _productRepository.UpdateProductAsync(product);
        }
    }
}