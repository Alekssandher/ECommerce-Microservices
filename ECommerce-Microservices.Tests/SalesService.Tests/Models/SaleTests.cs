using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SalesService.Models;
using Services.Models;

namespace SalesService.Tests.Models
{
    public sealed class SaleTests
    {
        [Fact]
        public void GivenAllParameters_ThenShouldSetThePropertiesCorrectely()
        {
            var expectedId = 1;
            var expectedCreatedAt = DateTime.UtcNow;
            var expectedCustomerId = 2;
            var expectedItems = new List<SaleItem>();
            
            var sale = new Sale
            {
                Id = expectedId,
                CreatedAt = expectedCreatedAt,
                CustomerId = expectedCustomerId,
                Items = expectedItems
            };
            
            Assert.Equal(expectedId, sale.Id);
            Assert.Equal(expectedCreatedAt, sale.CreatedAt);
            Assert.Equal(expectedCustomerId, sale.CustomerId);
            Assert.Equal(expectedItems, sale.Items);
        }

        [Fact]
        public void NewSale_Should_HaveDefaultStatusPending()
        {
            var sale = new Sale();
            Assert.Equal(SaleStatus.Pending, sale.Status);
        }

        [Fact]
        public void NewSale_Should_HaveCreatedAtUtcNow()
        {
            var before = DateTime.UtcNow;
            var sale = new Sale();
            var after = DateTime.UtcNow;

            Assert.InRange(sale.CreatedAt, before, after);
        }

        [Fact]
        public void NewSale_Should_HaveEmptyItemsList()
        {
            var sale = new Sale();
            Assert.NotNull(sale.Items);
            Assert.Empty(sale.Items);
        }

        [Fact]
        public void CanSetCustomerId()
        {
            var sale = new Sale { CustomerId = 123 };
            Assert.Equal(123, sale.CustomerId);
        }

        [Fact]
        public void CanChangeStatus()
        {
            var sale = new Sale { Status = SaleStatus.Confirmed };
            Assert.Equal(SaleStatus.Confirmed, sale.Status);
        }
    }   
}