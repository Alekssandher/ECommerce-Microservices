using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SalesService.DTOs;
using SalesService.Models;
using Services.Models;
using Shared.Messages;

namespace SalesService.Mappers
{
    internal static class SaleMapper
    {
        public static Sale ToSaleModel(this SaleRequest saleRequest)
        {
            return new Sale
            {
                CustomerId = saleRequest.CustomerId,
                CreatedAt = DateTime.UtcNow,
                Items = ToModelList(saleRequest.Items)

            };
        }

        public static SaleResponse ToSaleResponse(this Sale sale)
        {
            return new SaleResponse
            {
                Id = sale.Id,
                CustomerId = sale.CustomerId,
                Status = sale.Status.ToString(),
                CreatedAt = sale.CreatedAt,
                Items = sale.Items.ToResponseList()
            };
        }

        private static List<ItemResponse> ToResponseList(this List<SaleItem> saleItems)
        {
            if (saleItems.Count <= 0)
                return [];

            return [.. saleItems.Select(item => new ItemResponse{
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = Math.Round(item.Price, 2)
            })];
        }
        public static List<SaleResponse> ToSaleResponseList(this List<Sale> sales)
        {
            if (sales.Count <= 0)
                return [];

            return [.. sales.Select(sale => sale.ToSaleResponse())];
        }
        private static List<SaleItem> ToModelList(this List<ItemRequest> items)
        {
            if (items.Count <= 0)
                return [];

            return [.. items.Select(item => new SaleItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity
            } )];
        }
        public static SaleCreated ToSaleCreated(this Sale sale)
        {
            return new SaleCreated(
                sale.Id,
                sale.CustomerId,
                sale.Items.ToItemsReserved()
            );
        }


        private static List<ItemReserved> ToItemsReserved(this List<SaleItem> Items)
        {
            if (Items.Count <= 0)
                return [];

            return [.. Items.Select( i => new ItemReserved{
                SaleId = i.SaleId,
                ProductId = i.ProductId,
                Quantity = i.Quantity
            }).ToList()];
        }

        public static SaleRequest ToRequest(this SaleItemsReservedResponse reserved)
        {
            return new SaleRequest
            {
                CustomerId = reserved.CustomerId,
                Items = reserved.ItemsReserved?.Select(i => new ItemRequest
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity
                }).ToList() ?? new List<ItemRequest>()
            };
        }
    }
}