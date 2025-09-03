using MassTransit;
using SalesService.DTOs;
using SalesService.Services.Interfaces;
using Shared.Messages;
using Shared.ModelViews;

namespace SalesService.Consumers
{
    // internal class SaleCreateConsumer : IConsumer<SaleRequest>
    // {
    //     private readonly ISalesService _salesService;

    //     public SaleCreateConsumer(ISalesService salesService)
    //     {
    //         _salesService = salesService;
    //     }

    //     public async Task Consume(ConsumeContext<SaleRequest> context)
    //     {
    //         var request = context.Message;
            
    //         try
    //         {
    //             await _salesService.CreateSaleAsync(request);
                
    //             await context.RespondAsync(new { Success = true, Message = "Sale created successfully" });
    //         }
    //         catch (Exception ex)
    //         {
    //             await context.RespondAsync(new { Success = false, Error = ex.Message });
    //         }
    //     }
    // }
}