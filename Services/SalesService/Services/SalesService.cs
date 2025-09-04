using MassTransit;
using SalesService.DTOs;
using SalesService.Mappers;
using SalesService.Models;
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
        private readonly ILogger<SalesService> _logger;
        public SalesService(ISalesRepository salesRepository, IPublishEndpoint publishEndpoint, ILogger<SalesService> logger)
        {
            _salesRepository = salesRepository;
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task<int> CreateSaleAsync(SaleItemsReservedResponse request)
        {
            var sale = await _salesRepository.CreateSaleAsync(request.ToModel());

            return sale.Id;
        }

        public async Task SendSaleAsync(SaleRequest request)
        {
            if (request.CustomerId <= 0)
                throw new Exceptions.BadRequestException("Customer ID must be greater than zero");

            if (request.Items.Count <= 0)
                throw new Exceptions.BadRequestException("Sale must contain at least one item");

            foreach (var item in request.Items)
            {
                if (item.ProductId <= 0)
                    throw new Exceptions.BadRequestException($"Invalid Product ID: {item.ProductId}");

            }

            var sale = request.ToSaleModel();

            await _publishEndpoint.Publish(sale.ToSaleCreated());
            
        }

        public async Task CancelSaleAsync(int saleId)
        {
            var sale = await _salesRepository.GetByIdAsync(saleId)
                ?? throw new Exceptions.NotFoundException($"Sale with ID: {saleId} Not Found.");

            await _salesRepository.DeleteSaleAsync(sale);
        }

        public async Task ConfirmSaleAsync(int saleId)
        {
            var sale = await _salesRepository.GetByIdAsync(saleId)
                ?? throw new Exceptions.NotFoundException($"Sale with ID: {saleId} Not Found.");

            await _salesRepository.ConfirmSaleAsync(sale);

            foreach (var item in sale.Items)
            {
                
                await _publishEndpoint.Publish(new SaleConfirmed(
                    item.ProductId,
                    item.Quantity
                ));
                    
            }         
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

        public async Task UnauthorizeSale(int saleId)
        {
            var sale = await _salesRepository.GetByIdAsync(saleId)
                ?? throw new Exceptions.NotFoundException($"Sale with ID: {saleId} Not Found.");

            await _salesRepository.UnauthorizeSale(sale);
        }

    }
}