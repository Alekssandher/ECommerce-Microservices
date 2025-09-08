using MassTransit;
using Serilog;
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

            Log.Information("Received SaleCanceled message - MessageId: {MessageId}, CorrelationId: {CorrelationId}, StockItemId: {StockItemId}, Quantity: {Quantity}",
                context.MessageId, context.CorrelationId, message.StockItemId, message.Quantity);

            if (message == null || message.StockItemId <= 0 || message.Quantity <= 0)
            {
                Log.Warning("Invalid SaleCanceled message received - MessageId: {MessageId}, StockItemId: {StockItemId}, Quantity: {Quantity}",
                    context.MessageId, message?.StockItemId ?? 0, message?.Quantity ?? 0);
                return; 
            }

            try
            {
                Log.Information("Releasing stock - StockItemId: {StockItemId}, Quantity: {Quantity}, MessageId: {MessageId}",
                    message.StockItemId, message.Quantity, context.MessageId);

                await _stockService.ReleaseStockAsync(message.StockItemId, message.Quantity);

                Log.Information("Stock released successfully - StockItemId: {StockItemId}, Quantity: {Quantity}, MessageId: {MessageId}",
                    message.StockItemId, message.Quantity, context.MessageId);

                Log.Information("Publishing StockCanceled event - StockItemId: {StockItemId}, Quantity: {Quantity}, MessageId: {MessageId}",
                    message.StockItemId, message.Quantity, context.MessageId);

                await _publishEndpoint.Publish(new StockCanceled
                {
                    StockItemId = message.StockItemId,
                    Quantity = message.Quantity
                });

                Log.Information("StockCanceled event published successfully - StockItemId: {StockItemId}, Quantity: {Quantity}, MessageId: {MessageId}",
                    message.StockItemId, message.Quantity, context.MessageId);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to process SaleCanceled - StockItemId: {StockItemId}, Quantity: {Quantity}, MessageId: {MessageId}, Error: {Error}",
                    message.StockItemId, message.Quantity, context.MessageId, ex.Message);
                throw; 
            }
        }
    }
}