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
        private readonly IPublishEndpoint _publishEndpoint;

        public SaleCreatedConsumer(IStockManagementService stockService, IPublishEndpoint publishEndpoint)
        {
            _stockService = stockService;
            _publishEndpoint = publishEndpoint;
         
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

            await _publishEndpoint.Publish(
                message.ToReservedResponse()
            );
            
        }
    }
}