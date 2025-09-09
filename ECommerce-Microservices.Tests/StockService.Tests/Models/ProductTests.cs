using StockService.Models;

namespace StockService.Tests.Models
{
    public sealed class ProductTests
    {
        [Fact]
        public void CanSetId()
        {
            var product = new Product { Name = "Test", Description = "Desc", Price = 10.5m };
            typeof(Product).GetProperty(nameof(Product.Id))!.SetValue(product, 42);
            Assert.Equal(42, product.Id);
        }

        [Fact]
        public void CanSetName()
        {
            var product = new Product { Name = "Laptop", Description = "Gaming", Price = 2000m };
            Assert.Equal("Laptop", product.Name);
        }

        [Fact]
        public void CanSetDescription()
        {
            var product = new Product { Name = "Mouse", Description = "Wireless", Price = 150m };
            Assert.Equal("Wireless", product.Description);
        }

        [Fact]
        public void CanSetPrice()
        {
            var product = new Product { Name = "Keyboard", Description = "Mechanical", Price = 350m };
            Assert.Equal(350m, product.Price);
        }

        [Fact]
        public void NewProduct_Should_HaveDefaultValues()
        {
            var product = new Product { Name = string.Empty, Description = string.Empty, Price = 0m };
            Assert.Equal(0, product.Id);
            Assert.Equal(string.Empty, product.Name);
            Assert.Equal(string.Empty, product.Description);
            Assert.Equal(0m, product.Price);
        }
    }
}