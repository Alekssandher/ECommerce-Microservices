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

        public async Task CreateSaleAsync(SaleRequest request)
        {
            if (request.CustomerId <= 0)
                throw new Exceptions.BadRequestException("Customer ID must be greater than zero");

            if (!request.Items.Any())
                throw new Exceptions.BadRequestException("Sale must contain at least one item");

            foreach (var item in request.Items)
            {
                if (item.ProductId <= 0)
                    throw new Exceptions.BadRequestException($"Invalid Product ID: {item.ProductId}");
                
                if (item.Quantity <= 0)
                    throw new Exceptions.BadRequestException($"Quantity must be greater than zero for Product ID: {item.ProductId}");
                
                if (item.Price <= 0)
                    throw new Exceptions.BadRequestException($"Price must be greater than zero for Product ID: {item.ProductId}");
            }

            var sale = await _salesRepository.CreateSaleAsync(request.ToSaleModel());

            foreach (var item in sale.Items)
            {
                try
                {
                    await _publishEndpoint.Publish(new SaleCreated(
                        OrderId: sale.Id,
                        ProductId: item.ProductId,
                        Quantity: item.Quantity
                    ));
                }
                catch (Exception ex)
                {
                    await _publishEndpoint.Publish(new SaleCreationFailed(
                        CustomerId: sale.CustomerId,
                        ProductId: item.ProductId,
                        Quantity: item.Quantity,
                        Reason: $"Failed to publish sale creation event: {ex.Message}"
                    ));

                    // Log do erro seria ideal aqui
                    // _logger.LogError(ex, "Failed to publish SaleCreated event for Sale {SaleId}, Product {ProductId}", 
                    //     sale.Id, item.ProductId);
                }
            }
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

        public async Task<List<SaleResponse>> GetAllSalesAsync()
        {
            var sales = await _salesRepository.GetAllAsync();
            return sales.ToSaleResponseList();
        }

        public async Task<SaleResponse> GetSaleByIdAsync(int saleId)
        {
            var sale = await _salesRepository.GetByIdAsync(saleId)
                ?? throw new Exceptions.NotFoundException($"Sale with ID: {saleId} Not Found.");

            return sale.ToSaleResponse();
        }
    }
}