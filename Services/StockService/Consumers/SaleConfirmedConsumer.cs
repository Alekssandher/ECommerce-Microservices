using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Shared.Messages;
using Shared.DTOs;
using StockService.Services;

namespace StockService.Consumers
{
    public class SaleConfirmedConsumer : IConsumer<SaleConfirmed>
    {
        private readonly IStockManagementService _stockService;
        private readonly IPublishEndpoint _publishEndpoint;

        public SaleConfirmedConsumer(IStockManagementService stockService, IPublishEndpoint publishEndpoint)
        {
            _stockService = stockService;
            _publishEndpoint = publishEndpoint;

        }

        public async Task Consume(ConsumeContext<SaleConfirmed> context)
        {
            var message = context.Message;

            try
            {
                await _stockService.RemoveStockAsync(message.StockItemId, message.Quantity);
            }
            catch (Exception)
            {
                
                throw;
            }

            await _publishEndpoint.Publish(new StockReleased
            {
                StockItemId = message.StockItemId,
                Quantity = message.Quantity,
                ReleasedAt = DateTime.UtcNow
            });
        }
    }
}