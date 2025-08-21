using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StockService.Dtos;
using StockService.Models;

namespace StockService.Mappers
{
    public static class ProductMapper
    {
        public static Product ToModel(this ProductRequest productRequest)
        {
            return new Product
            {
                Name = productRequest.Name,
                Description = productRequest.Description,
                Price = productRequest.Price,
                Quantity = productRequest.Quantity
            };
        }

        public static ProductResponse ToResponse(this Product product)
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

        public static List<ProductResponse> ToProductList(this List<Product> products)
        {
            if (products == null || products.Count == 0)
                return [];

            return [.. products.Select(product => new ProductResponse{
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Quantity = product.Quantity
            })];

        }
    }
}