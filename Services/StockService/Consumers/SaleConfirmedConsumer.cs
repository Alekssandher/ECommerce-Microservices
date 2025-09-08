using MassTransit;
using Shared.Messages;
using Shared.DTOs;
using StockService.Services;
using Serilog;


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

            Log.Information("Received SaleConfirmed message - MessageId: {MessageId}, CorrelationId: {CorrelationId}, StockItemId: {StockItemId}, Quantity: {Quantity}",
                context.MessageId, context.CorrelationId, message.StockItemId, message.Quantity);

            if (message == null || message.StockItemId <= 0 || message.Quantity <= 0)
            {
                Log.Warning("Invalid SaleConfirmed message received - MessageId: {MessageId}, StockItemId: {StockItemId}, Quantity: {Quantity}",
                    context.MessageId, message?.StockItemId ?? 0, message?.Quantity ?? 0);
                return; 
            }

            try
            {
        
                Log.Information("Removing stock - StockItemId: {StockItemId}, Quantity: {Quantity}, MessageId: {MessageId}",
                    message.StockItemId, message.Quantity, context.MessageId);

                await _stockService.RemoveStockAsync(message.StockItemId, message.Quantity);

                Log.Information("Stock removed successfully - StockItemId: {StockItemId}, Quantity: {Quantity}, MessageId: {MessageId}",
                    message.StockItemId, message.Quantity, context.MessageId);

                Log.Information("Publishing StockReleased event - StockItemId: {StockItemId}, Quantity: {Quantity}, ReleasedAt: {ReleasedAt}, MessageId: {MessageId}",
                    message.StockItemId, message.Quantity, DateTime.UtcNow, context.MessageId);

                await _publishEndpoint.Publish(new StockReleased
                {
                    StockItemId = message.StockItemId,
                    Quantity = message.Quantity,
                    ReleasedAt = DateTime.UtcNow
                });

                Log.Information("StockReleased event published successfully - StockItemId: {StockItemId}, Quantity: {Quantity}, ReleasedAt: {ReleasedAt}, MessageId: {MessageId}",
                    message.StockItemId, message.Quantity, DateTime.UtcNow, context.MessageId);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to process SaleConfirmed - StockItemId: {StockItemId}, Quantity: {Quantity}, MessageId: {MessageId}, Error: {Error}",
                    message.StockItemId, message.Quantity, context.MessageId, ex.Message);
                throw; 
            }
        }
    }
}