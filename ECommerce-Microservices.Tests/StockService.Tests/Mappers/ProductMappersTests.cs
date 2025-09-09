using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StockService.Dtos;
using StockService.Mappers;
using StockService.Models;

namespace StockService.Tests.Mappers
{
    public sealed class ProductkMappersTests
    {
        [Fact]
        public void ToProductModel_ShouldMapFieldsCorrectly()
        {
            // Arrange
            var request = new CreateProductRequest
            {
                Name = "Notebook",
                Description = "Gaming laptop",
                Price = 4500.99m,
                InitialQuantity = 0
            };

            // Act
            var product = request.ToProductModel();

            // Assert
            Assert.Equal("Notebook", product.Name);
            Assert.Equal("Gaming laptop", product.Description);
            Assert.Equal(4500.99m, product.Price);
            Assert.Equal(0, product.Id); 
        }

        [Fact]
        public void ToProductResponse_ShouldMapFieldsCorrectly()
        {
            // Arrange
            var product = new Product
            {
                Id = 10,
                Name = "Mouse",
                Description = "Wireless",
                Price = 150.75m
            };

            // Act
            var response = product.ToProductResponse();

            // Assert
            Assert.Equal(10, response.Id);
            Assert.Equal("Mouse", response.Name);
            Assert.Equal("Wireless", response.Description);
            Assert.Equal(150.75m, response.Price);
        }

        [Fact]
        public void ToProductResponseList_ShouldReturnMappedList()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product { Id = 1, Name = "Keyboard", Description = "Mechanical", Price = 300m },
                new Product { Id = 2, Name = "Monitor", Description = "4K", Price = 1200m }
            };

            // Act
            var responses = products.ToProductResponseList();

            // Assert
            Assert.Equal(2, responses.Count);

            Assert.Equal(1, responses[0].Id);
            Assert.Equal("Keyboard", responses[0].Name);
            Assert.Equal("Mechanical", responses[0].Description);
            Assert.Equal(300m, responses[0].Price);

            Assert.Equal(2, responses[1].Id);
            Assert.Equal("Monitor", responses[1].Name);
            Assert.Equal("4K", responses[1].Description);
            Assert.Equal(1200m, responses[1].Price);
        }

        [Fact]
        public void ToProductResponseList_ShouldReturnEmptyList_WhenNull()
        {
            // Arrange
            List<Product>? products = null;

            // Act
            var responses = products?.ToProductResponseList();

            // Assert
            Assert.Null(responses);
        }

        [Fact]
        public void ToProductResponseList_ShouldReturnEmptyList_WhenEmpty()
        {
            // Arrange
            var products = new List<Product>();

            // Act
            var responses = products.ToProductResponseList();

            // Assert
            Assert.Empty(responses);
        } 
    }
}