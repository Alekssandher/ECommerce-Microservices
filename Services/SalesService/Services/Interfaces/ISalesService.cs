using SalesService.DTOs;

namespace SalesService.Services.Interfaces
{
    internal interface ISalesService
    {
        Task CreateSaleAsync(SaleRequest request);
        Task<SaleResponse> GetSaleByIdAsync(int saleId);
        Task<List<SaleResponse>> GetAllSalesAsync();
        Task ConfirmSaleAsync(int saleId);
        Task CancelSaleAsync(int saleId);
    }
}