using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StockService.Models;

namespace StockService.Tests.Models
{
    public sealed class StockItemTests
    {
        [Fact]
        public void CanSetProductId()
        {
            var stock = new StockItem { ProductId = 5 };
            Assert.Equal(5, stock.ProductId);
        }

        [Fact]
        public void CanSetQuantities()
        {
            var stock = new StockItem { QuantityAvailable = 100, QuantityReserved = 20 };
            Assert.Equal(100, stock.QuantityAvailable);
            Assert.Equal(20, stock.QuantityReserved);
        }

        [Fact]
        public void CanSetProductNavigation()
        {
            var product = new Product { Name = "Tablet", Description = "10 inch", Price = 999.99m };
            var stock = new StockItem { ProductId = 7, Product = product };

            Assert.Equal(7, stock.ProductId);
            Assert.Equal(product, stock.Product);
            Assert.Equal("Tablet", stock.Product.Name);
        }

        [Fact]
        public void NewStockItem_Should_HaveDefaultValues()
        {
            var stock = new StockItem();
            Assert.Equal(0, stock.ProductId);
            Assert.Equal(0, stock.QuantityAvailable);
            Assert.Equal(0, stock.QuantityReserved);
            Assert.Null(stock.Product); 
        }
    }
}