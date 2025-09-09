using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using SalesService.DTOs;
using SalesService.Mappers;
using SalesService.Models;
using Services.Models;
using Shared.Messages;

namespace SalesService.Tests.Mappers
{
    public class SaleMapperTests
    {
        private readonly Faker _faker = new();
        
        [Fact]
        public void ToSaleModel_ShouldMapFieldsCorrectly()
        {
            // Arrange
            var request = new SaleRequest
            {
                CustomerId = _faker.Random.Int(1, 1000),
                Items =
                [
                    new() {
                        ProductId = _faker.Random.Int(1, 100),
                        Quantity = _faker.Random.Int(1, 10)
                    }
                ]
            };

            // Act
            var sale = request.ToSaleModel();

            // Assert
            Assert.Equal(request.CustomerId, sale.CustomerId);
            Assert.NotNull(sale.Items);
            Assert.Single(sale.Items);
            Assert.Equal(request.Items[0].ProductId, sale.Items[0].ProductId);
            Assert.Equal(request.Items[0].Quantity, sale.Items[0].Quantity);
            Assert.True((DateTime.UtcNow - sale.CreatedAt).TotalSeconds < 1);
        }

        [Fact]
        public void ToSaleModel_ShouldHandleEmptyItems()
        {
            // Arrange
            var request = new SaleRequest
            {
                CustomerId = 42,
                Items = [] // lista vazia
            };

            // Act
            var sale = request.ToSaleModel();

            // Assert
            Assert.NotNull(sale.Items);
            Assert.Empty(sale.Items);
        }

        [Fact]
        public void ToSaleResponse_ShouldMapFieldsCorrectly()
        {
            // Arrange
            var sale = new Sale
            {
                Id = _faker.Random.Int(1, 1000),
                CustomerId = _faker.Random.Int(1, 1000),
                Status = SaleStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                Items =
                [
                    new() {
                        ProductId = _faker.Random.Int(1, 100),
                        Quantity = _faker.Random.Int(1, 10),
                        Price = _faker.Random.Decimal(1, 100)
                    }
                ]
            };

            // Act
            var response = sale.ToSaleResponse();

            // Assert
            Assert.Equal(sale.Id, response.Id);
            Assert.Equal(sale.CustomerId, response.CustomerId);
            Assert.Equal(sale.Status.ToString(), response.Status);
            Assert.Single(response.Items);
            Assert.Equal(Math.Round(sale.Items[0].Price, 2), response.Items[0].UnitPrice);
        }

        [Fact]
        public void ToSaleResponseList_ShouldMapAllSales()
        {
            // Arrange
            var sales = new List<Sale>
            {
                new() { Id = 1, CustomerId = 1, Status = SaleStatus.Pending, Items = [] },
                new() { Id = 2, CustomerId = 2, Status = SaleStatus.Confirmed, Items = [] }
            };

            // Act
            var responses = sales.ToSaleResponseList();

            // Assert
            Assert.Equal(2, responses.Count);
            Assert.Equal(1, responses[0].Id);
            Assert.Equal("Pending", responses[0].Status);
            Assert.Equal(2, responses[1].Id);
            Assert.Equal("Confirmed", responses[1].Status);
        }

        [Fact]
        public void ToModel_ShouldMapReservedItems()
        {
            // Arrange
            var reserved = new SaleItemsReservedResponse
            {
                CustomerId = _faker.Random.Int(1, 1000),
                ItemsReserved =
                [
                    new() {
                        SaleId = _faker.Random.Int(1, 1000),
                        ProductId = _faker.Random.Int(1, 100),
                        Quantity = _faker.Random.Int(1, 10),
                        Price = _faker.Random.Decimal(1, 100)
                    }
                ]
            };

            // Act
            var sale = reserved.ToModel();

            // Assert
            Assert.Equal(reserved.CustomerId, sale.CustomerId);
            Assert.Single(sale.Items);
            Assert.Equal(reserved.ItemsReserved[0].ProductId, sale.Items[0].ProductId);
            Assert.Equal(reserved.ItemsReserved[0].Quantity, sale.Items[0].Quantity);
            Assert.Equal(reserved.ItemsReserved[0].Price, sale.Items[0].Price);
            Assert.Equal(reserved.ItemsReserved[0].SaleId, sale.Items[0].SaleId);
        }
        
    }
}