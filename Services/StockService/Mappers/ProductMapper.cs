using StockService.Dtos;
using StockService.Models;

namespace StockService.Mappers
{
    public static class ProductMapper
    {
        public static Product ToProductModel(this CreateProductRequest request)
        {
            return new Product
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                Quantity = request.InitialQuantity
            };
        }

        public static ProductResponse ToProductResponse(this Product product)
        {
            return new ProductResponse
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Quantity = product.Quantity
            };
        }

        public static List<ProductResponse> ToProductResponseList(this List<Product> products)
        {
            if (products == null || products.Count == 0)
                return new List<ProductResponse>();

            return products.Select(product => product.ToProductResponse()).ToList();
        }
    }
}