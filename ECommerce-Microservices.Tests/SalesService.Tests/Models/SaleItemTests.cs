using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Services.Models;

namespace SalesService.Tests.Models
{
    public sealed class SaleItemTests
    {
        [Fact]
        public void CanSetSaleId()
        {
            var item = new SaleItem { SaleId = 10 };
            Assert.Equal(10, item.SaleId);
        }

        [Fact]
        public void CanSetProductId()
        {
            var item = new SaleItem { ProductId = 555 };
            Assert.Equal(555, item.ProductId);
        }

        [Fact]
        public void CanSetQuantity()
        {
            var item = new SaleItem { Quantity = 3 };
            Assert.Equal(3, item.Quantity);
        }

        [Fact]
        public void CanSetPrice()
        {
            var item = new SaleItem { Price = 49.99m };
            Assert.Equal(49.99m, item.Price);
        }

        [Fact]
        public void NewSaleItem_Should_HaveDefaultValues()
        {
            var item = new SaleItem();
            Assert.Equal(0, item.SaleId);
            Assert.Equal(0, item.ProductId);
            Assert.Equal(0, item.Quantity);
            Assert.Equal(0m, item.Price);
        }
    }
}