using MassTransit;
using SalesService.DTOs;
using SalesService.Mappers;
using SalesService.Repositories.Interfaces;
using SalesService.Services.Interfaces;
using Shared.Exceptions;

namespace SalesService.Services
{
    internal class SalesService : ISalesService
    {
        private readonly ISalesRepository _salesRepository;

        public SalesService(ISalesRepository salesRepository)
        {
            _salesRepository = salesRepository;
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
            await _salesRepository.CreateSaleAsync(request.ToSaleModel());
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