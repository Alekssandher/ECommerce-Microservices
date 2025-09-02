using MassTransit;
using SalesService.DTOs;
using SalesService.Services.Interfaces;
using Shared.Messages;
using Shared.ModelViews;

namespace SalesService.Consumers
{
    internal class SaleCreateConsumer : IConsumer<SaleRequest>
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ISalesService _salesService;

        public SaleCreateConsumer(IPublishEndpoint publishEndpoint, ISalesService salesService)
        {
            _publishEndpoint = publishEndpoint;
            _salesService = salesService;
        }

        public async Task Consume(ConsumeContext<SaleRequest> context)
        {
            var message = context.Message;

            try
            {
                await _publishEndpoint.Publish(new SaleConfirmed(message.CustomerId, message.ProductId, message.Quantity));
            }
            catch (Exception ex)
            {
                await _publishEndpoint.Publish(new SaleCreationFailed(
                    message.CustomerId,
                    message.ProductId,
                    message.Quantity,
                    ex.Message
                    )
                );
               
            }
            
        }
    }
}