using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Shared.Exceptions;
using Shared.Messages;
using Shared.ModelViews;
using StockService.Consumers.DTOs;
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

            try
            {
                await _stockService.ReserveStockAsync(message.ProductId, message.Quantity);
                
                await context.Publish(new StockReserved
                (
                    message.SaleId,
                    message.ProductId,
                    message.Quantity
                   
                ));
            }
            catch (Exception ex)
            {

                await context.Publish(new StockReservationFailed
                (
                    message.ProductId,
                    message.Quantity,
                    ex.Message
                ));
            }
            ;
        }
    }
}