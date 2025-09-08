using MassTransit;
using Serilog;
using Shared.Messages;
using Shared.ModelViews;
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

            Log.Information("Received SaleCreated message - MessageId: {MessageId}, CorrelationId: {CorrelationId}, SaleId: {SaleId}, CustomerId: {CustomerId}, ItemCount: {ItemCount}",
                context.MessageId, context.CorrelationId, message.SaleId, message.CustomerId, message.Items?.Count ?? 0);

            if (message == null || message.CustomerId <= 0 || message.Items == null || message.Items.Count == 0)
            {
                Log.Warning("Invalid SaleCreated message received - MessageId: {MessageId}, SaleId: {SaleId}, CustomerId: {CustomerId}, ItemCount: {ItemCount}",
                    context.MessageId, message?.SaleId, message?.CustomerId ?? 0, message?.Items?.Count ?? 0);
                return; 
            }

            var items = message.Items
                .Select(i => (productId: i.ProductId, quantity: i.Quantity))
                .ToList();

            try
            {
                Log.Information("Reserving stock items - SaleId: {SaleId}, CustomerId: {CustomerId}, ItemCount: {ItemCount}, MessageId: {MessageId}",
                    message.SaleId, message.CustomerId, items.Count, context.MessageId);

                await _stockService.ReserveStockItemsAsync(items);

                Log.Information("Stock items reserved successfully - SaleId: {SaleId}, CustomerId: {CustomerId}, ItemCount: {ItemCount}, MessageId: {MessageId}",
                    message.SaleId, message.CustomerId, items.Count, context.MessageId);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to reserve stock items - SaleId: {SaleId}, CustomerId: {CustomerId}, ItemCount: {ItemCount}, MessageId: {MessageId}, Error: {Error}",
                    message.SaleId, message.CustomerId, items.Count, context.MessageId, ex.Message);

                Log.Information("Publishing SaleCreationFailed event due to stock reservation failure - SaleId: {SaleId}, CustomerId: {CustomerId}, MessageId: {MessageId}",
                    message.SaleId, message.CustomerId, context.MessageId);

                await _publishEndpoint.Publish(new SaleCreationFailed(message.CustomerId, message.SaleId, ex.Message));
                return;
            }

            Log.Information("Fetching prices for items - SaleId: {SaleId}, CustomerId: {CustomerId}, ItemCount: {ItemCount}, MessageId: {MessageId}",
                message.SaleId, message.CustomerId, items.Count, context.MessageId);

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

            Log.Information("Prices fetched successfully for items - SaleId: {SaleId}, CustomerId: {CustomerId}, ItemCount: {ItemCount}, MessageId: {MessageId}",
                message.SaleId, message.CustomerId, itemsWithPrices.Length, context.MessageId);

            Log.Information("Publishing SaleItemsReservedResponse event - SaleId: {SaleId}, CustomerId: {CustomerId}, ItemCount: {ItemCount}, MessageId: {MessageId}",
                message.SaleId, message.CustomerId, itemsWithPrices.Length, context.MessageId);

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

            Log.Information("SaleItemsReservedResponse event published successfully - SaleId: {SaleId}, CustomerId: {CustomerId}, ItemCount: {ItemCount}, MessageId: {MessageId}",
                message.SaleId, message.CustomerId, itemsWithPrices.Length, context.MessageId);
        }
    }
}