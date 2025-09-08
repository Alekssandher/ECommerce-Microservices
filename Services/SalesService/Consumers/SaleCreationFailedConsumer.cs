using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using SalesService.Services.Interfaces;
using Serilog;
using Shared.Messages;
using Shared.ModelViews;

namespace SalesService.Consumers
{
    public class SaleCreationFailedConsumer : IConsumer<SaleCreationFailed>
    {
        private readonly ISalesService _salesService;

        public SaleCreationFailedConsumer(ISalesService salesService)
        {
            _salesService = salesService;
        }

        public async Task Consume(ConsumeContext<SaleCreationFailed> context)
        {
            var message = context.Message;

            Log.Information("Received SaleCreationFailed message - MessageId: {MessageId}, CorrelationId: {CorrelationId}, SaleId: {SaleId}, CustomerId: {CustomerId}",
                context.MessageId, context.CorrelationId, message.SaleId, message.CustomerId);

            if (message == null )
            {
                Log.Warning("Invalid or empty SaleCreationFailed message received - MessageId: {MessageId}, Skipping processing",
                    context.MessageId);
                return; 
            }

            try
            {
                Log.Information("Attempting to recreate sale for failed attempt - SaleId: {SaleId}, CustomerId: {CustomerId}, MessageId: {MessageId}",
                    message.SaleId, message.CustomerId, context.MessageId);

                var saleId = await _salesService.CreateSaleAsync(new SaleItemsReservedResponse
                {
                    SaleId = message.SaleId,
                    CustomerId = message.CustomerId,
                    ItemsReserved = []
                });

                Log.Information("Sale recreated successfully - OriginalSaleId: {OriginalSaleId}, CreatedSaleId: {CreatedSaleId}, MessageId: {MessageId}",
                    message.SaleId, saleId, context.MessageId);

                Log.Information("Attempting to unauthorize sale - SaleId: {SaleId}, MessageId: {MessageId}",
                    saleId, context.MessageId);

                await _salesService.UnauthorizeSale(saleId);

                Log.Information("Sale unauthorized successfully - SaleId: {SaleId}, MessageId: {MessageId}",
                    saleId, context.MessageId);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to process SaleCreationFailed - SaleId: {SaleId}, CustomerId: {CustomerId}, MessageId: {MessageId}, Error: {Error}",
                    message.SaleId, message.CustomerId, context.MessageId, ex.Message);
                throw; 
            }
        }
    }
}