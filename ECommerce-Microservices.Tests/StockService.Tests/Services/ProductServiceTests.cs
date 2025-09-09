using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using Moq;
using Shared.Exceptions;
using StockService.Dtos;
using StockService.Models;
using StockService.Repositories;
using StockService.Services;

namespace StockService.Tests.Services
{
    public class ProductServiceTests
    {
        private readonly Mock<IProductRepository> _productRepositoryMock;
        private readonly ProductService _productService;
        public ProductServiceTests()
        {

            _productRepositoryMock = new Mock<IProductRepository>();
            _productService = new ProductService(_productRepositoryMock.Object);
        }

        [Fact]
        public async Task CreateProductAsync_ShouldCallRepository()
        {
            // Arrange
            var request = new CreateProductRequest
            {
                Name = "Test Poduct",
                Description = "Product Description",
                Price = 10m,
                InitialQuantity = 5
            };

            // Act
            await _productService.CreateProductAsync(request);

            // Assert
            _productRepositoryMock.Verify(r => r.CreateProductAsync(
                It.Is<Product>(p => p.Name == request.Name && p.Price == request.Price),
                request.InitialQuantity
            ), Times.Once);
        }

        [Fact]
        public async Task GetAllProductsAsync_ShouldReturnProductResponses()
        {
            // Arrange
            var products = new List<Product>
            {
                new() { Id = 1, Name = "P1", Price = 10, Description = string.Empty},
                new() { Id = 2, Name = "P2", Price = 20, Description = string.Empty }
            };

            _productRepositoryMock.Setup(r => r.GetAllProductsAsync()).ReturnsAsync(products);

            // Act
            var result = await _productService.GetAllProductsAsync();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal("P1", result[0].Name);
            Assert.Equal("P2", result[1].Name);
        }

        [Fact]
        public async Task GetProductByIdAsync_ShouldReturnProductResponse_WhenProductExists()
        {
            // Arrange
            var expectedName = "Test Product";
            var expectedDescription = "Product Description";

            var product = new Product { Id = 1, Name = expectedName, Price = 10, Description = expectedDescription };
            _productRepositoryMock.Setup(r => r.GetProductByIdAsync(1)).ReturnsAsync(product);

            // Act
            var result = await _productService.GetProductByIdAsync(1);

            // Assert
            Assert.Equal(1, result.Id);
            Assert.Equal(expectedName, result.Name);
            Assert.Equal(expectedDescription, result.Description);

        }

        [Fact]
        public async Task GetProductByIdAsync_ShouldThrowNotFoundException_WhenProductDoesNotExist()
        {
            // Arrange
            _productRepositoryMock.Setup(r => r.GetProductByIdAsync(1)).ReturnsAsync((Product?)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exceptions.NotFoundException>(() => _productService.GetProductByIdAsync(1));
        }

        [Fact]
        public async Task GetProductPriceByIdAsync_ShouldReturnPrice()
        {
            // Arrange
            _productRepositoryMock.Setup(r => r.GetProductPriceByIdAsync(1)).ReturnsAsync(15m);

            // Act
            var price = await _productService.GetProductPriceByIdAsync(1);

            // Assert
            Assert.Equal(15m, price);
        }

        [Fact]
        public async Task UpdateProductAsync_ShouldUpdateFields()
        {
            // Arrange
            var existingProduct = new Product { Id = 1, Name = "Old", Description = "OldDesc", Price = 10 };
            var request = new UpdateProductRequest { ProductId = 1, Name = "New", Price = 20 };

            _productRepositoryMock.Setup(r => r.GetProductByIdAsync(1)).ReturnsAsync(existingProduct);

            // Act
            await _productService.UpdateProductAsync(request);

            // Assert
            Assert.Equal("New", existingProduct.Name);
            Assert.Equal(20, existingProduct.Price);
            _productRepositoryMock.Verify(r => r.UpdateProductAsync(existingProduct), Times.Once);
        }

        [Fact]
        public async Task UpdateProductAsync_ShouldThrowNotFoundException_WhenProductDoesNotExist()
        {
            // Arrange
            var request = new UpdateProductRequest { ProductId = 1, Name = "New" };
            _productRepositoryMock.Setup(r => r.GetProductByIdAsync(1)).ReturnsAsync((Product?)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exceptions.NotFoundException>(() => _productService.UpdateProductAsync(request));
        }

        [Fact]
        public async Task UpdateProductAsync_ShouldThrowBadRequestException_WhenProductIdInvalid()
        {
            // Arrange
            var request = new UpdateProductRequest { ProductId = 0, Name = "New" };

            // Act & Assert
            await Assert.ThrowsAsync<Exceptions.BadRequestException>(() => _productService.UpdateProductAsync(request));
        }
    }
}