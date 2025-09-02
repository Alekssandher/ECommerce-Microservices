using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SalesService.DTOs;

namespace SalesService.Services.Interfaces
{
    internal interface ISalesService
    {
        Task<SaleResponse> CreateSaleAsync(SaleRequest request);
        Task<SaleResponse> GetSaleByIdAsync(int saleId);
        Task<List<SaleResponse>> GetAllSalesAsync();
        Task ConfirmSaleAsync(int saleId);
        Task CancelSaleAsync(int saleId);
    }
}