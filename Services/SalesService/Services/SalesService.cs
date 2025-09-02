using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using SalesService.DTOs;
using SalesService.Services.Interfaces;

namespace SalesService.Services
{
    internal class SalesService : ISalesService
    {
        private readonly IPublishEndpoint _publishEndpoint;

        public SalesService(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }
        
        public Task CancelSaleAsync(int saleId)
        {
            throw new NotImplementedException();
        }

        public Task ConfirmSaleAsync(int saleId)
        {
            throw new NotImplementedException();
        }

        public Task<SaleResponse> CreateSaleAsync(SaleRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<List<SaleResponse>> GetAllSalesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<SaleResponse> GetSaleByIdAsync(int saleId)
        {
            throw new NotImplementedException();
        }
    }
}