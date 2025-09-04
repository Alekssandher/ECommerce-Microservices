using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.Transports;
using Shared.Exceptions;
using Shared.Mappers;
using Shared.Messages;
using Shared.ModelViews;
using StockService.Consumers.DTOs;
using StockService.Services;

namespace StockService.Consumers
{
    public class SaleCreatedConsumer : IConsumer<SaleCreated>
    {
        private readonly IStockManagementService _stockService;
        private readonly IProductService _productService;
        private readonly IPublishEndpoint _publishEndpoint;

        public SaleCreatedConsumer(IStockManagementService stockService, IPublishEndpoint publishEndpoint, IProductService productService)
        {
            _stockService = stockService;
            _publishEndpoint = publishEndpoint;
            _productService = productService;
         
        }

        public async Task Consume(ConsumeContext<SaleCreated> context)
        {
            SaleCreated message = context.Message;

            var items = message.Items
                .Select(i => (productId: i.ProductId, quantity: i.Quantity))
                .ToList();


            try
            {
                await _stockService.ReserveStockItemsAsync(items);
            }
            catch (Exception ex)
            {
                await _publishEndpoint.Publish(new SaleCreationFailed(message.CustomerId, message.SaleId, ex.Message));
                Console.WriteLine("Reached SaleCreation");
                return;
            }
            
            var itemsWithPrices = await Task.WhenAll(
                message.Items.Select(async i =>
                {
                    var price = await _productService.GetProductPriceByIdAsync(i.ProductId); 
                    return new
                    {
                        i.ProductId,
                        i.Quantity,
                        i.SaleId,
                        Price = price
                    };
                })
            );
            await _publishEndpoint.Publish(new SaleItemsReservedResponse
            {
                SaleId = message.SaleId,
                CustomerId = message.CustomerId,
                ItemsReserved = [.. itemsWithPrices.Select(i => new ItemReserved
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    Price = i.Price, 
                    SaleId = i.SaleId
                })]
            });
            
        }
    }
}