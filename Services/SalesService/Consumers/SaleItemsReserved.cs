using MassTransit;
using SalesService.DTOs;
using SalesService.Mappers;
using SalesService.Services.Interfaces;
using Serilog;
using Shared.Messages;
using Shared.ModelViews;

namespace SalesService.Consumers
{
    internal class SaleItemsReserved : IConsumer<SaleItemsReservedResponse>
    {
        private readonly ISalesService _salesService;

        public SaleItemsReserved(ISalesService salesService)
        {
            _salesService = salesService;
        }

        public async Task Consume(ConsumeContext<SaleItemsReservedResponse> context)
        {
            var message = context.Message;

            if (message == null)
            {
                Log.Warning("Invalid or empty SaleItemsReserved message received - MessageId: {MessageId}, Skipping processing", context.MessageId);
                return;  
            }

            Log.Information("Received SaleItemsReserved message - MessageId: {MessageId}, CorrelationId: {CorrelationId}, SaleId: {SaleId}, Timestamp: {Timestamp}", 
            context.MessageId, context.CorrelationId, message.SaleId, DateTime.Now);
            
            try
            {
                await _salesService.CreateSaleAsync(message);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to create sale from reserved items - SaleId: {SaleId}, MessageId: {MessageId}, Error: {Error}", 
                    message.SaleId, context.MessageId, ex.Message);
                
                throw;  
            }
        }
    }
}