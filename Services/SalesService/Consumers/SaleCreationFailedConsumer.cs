using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using SalesService.Services.Interfaces;
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
            Console.WriteLine("Reached SaleFailed");

            await _salesService.UnauthorizeSale(message.SaleId);
        }
    }
}