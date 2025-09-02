using MassTransit;
using SalesService.DTOs;
using SalesService.Mappers;
using SalesService.Repositories.Interfaces;
using SalesService.Services.Interfaces;
using Shared.Exceptions;
using Shared.Messages;
using Shared.ModelViews;

namespace SalesService.Services
{
    internal class SalesService : ISalesService
    {
        private readonly ISalesRepository _salesRepository;
        private readonly IPublishEndpoint _publishEndpoint;
        public SalesService(ISalesRepository salesRepository, IPublishEndpoint publishEndpoint)
        {
            _salesRepository = salesRepository;
            _publishEndpoint = publishEndpoint;
        }

        public Task CancelSaleAsync(int saleId)
        {
            throw new NotImplementedException();
        }

        public async Task ConfirmSaleAsync(int saleId)
        {
            var sale = await _salesRepository.GetByIdAsync(saleId)
                ?? throw new Exceptions.NotFoundException($"Sale with ID: {saleId} Not Found.");

            await _salesRepository.ConfirmSaleAsync(sale);
        }

        public async Task CreateSaleAsync(SaleRequest request)
        {
            var sale = await _salesRepository.CreateSaleAsync(request.ToSaleModel());
            
            try
            {
                await _publishEndpoint.Publish(new SaleConfirmed(
                    CustomerId: sale.CustomerId,
                    StockItemId: sale.Items.First().ProductId, 
                    Quantity: sale.Items.First().Quantity
                ));
            }
            catch (Exception ex)
            {
                await _publishEndpoint.Publish(new SaleCreationFailed(
                    CustomerId: sale.CustomerId,
                    ProductId: sale.Items.First().ProductId,
                    Quantity: sale.Items.First().Quantity,
                    Reason: ex.Message
                ));
            }
        }

        public async Task<List<SaleResponse>> GetAllSalesAsync()
        {
            var res = await _salesRepository.GetAllAsync();

            return res.ToSaleResponseList();
        }

        public async Task<SaleResponse> GetSaleByIdAsync(int saleId)
        {
            var res = await _salesRepository.GetByIdAsync(saleId)
                ?? throw new Exceptions.NotFoundException($"Sale with ID: {saleId} Not Found.");

            return res.ToSaleResponse();
        }
    }
}