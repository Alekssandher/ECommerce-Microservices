using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Shared.Exceptions;
using Shared.Messages;
using StockService.Services;

namespace StockService.Consumers
{
    public class SaleCreatedConsumer : IConsumer<SaleCreated>
    {
        private readonly IStockManagementService _stockService;

        public SaleCreatedConsumer(IStockManagementService stockService, IProductService productService)
        {
            _stockService = stockService;
         
        }
        
        public async Task Consume(ConsumeContext<SaleCreated> context)
        {
            var message = context.Message;
            
            await _stockService.RemoveStockAsync(message.ProductId, message.Quantity);
        }
    }
}