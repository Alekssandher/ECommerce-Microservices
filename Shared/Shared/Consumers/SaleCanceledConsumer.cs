using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Shared.DTOs;
using StockService.Services;

namespace StockService.Consumers
{
    public class SaleCanceledConsumer : IConsumer<SaleCanceled>
    {
        private readonly IStockManagementService _stockService;
        private readonly IPublishEndpoint _publishEndpoint;

        public SaleCanceledConsumer(IStockManagementService stockService, IPublishEndpoint publishEndpoint)
        {
            _stockService = stockService;
            _publishEndpoint = publishEndpoint;

        }

        public async Task Consume(ConsumeContext<SaleCanceled> context)
        {
            var message = context.Message;

            await _stockService.ReleaseStockAsync(message.StockItemId, message.Quantity);

            await _publishEndpoint.Publish(new StockCanceled
            {
                StockItemId = message.StockItemId,
                Quantity = message.Quantity
            });

        }
    }
}