using MassTransit;
using SalesService.DTOs;
using SalesService.Mappers;
using SalesService.Services.Interfaces;
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
            SaleItemsReservedResponse message = context.Message;

            
            await _salesService.CreateSaleAsync(message);
        }
    }
}