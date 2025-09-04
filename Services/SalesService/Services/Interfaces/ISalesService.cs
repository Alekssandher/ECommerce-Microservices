using SalesService.DTOs;

namespace SalesService.Services.Interfaces
{
    public interface ISalesService
    {
        Task CreateSaleAsync(SaleRequest request);
        Task UnauthorizeSale(int saleId);
        Task SendSaleAsync(SaleRequest request);
        Task<SaleResponse> GetSaleByIdAsync(int saleId);
        Task<List<SaleResponse>> GetAllSalesAsync();
        Task ConfirmSaleAsync(int saleId);
        Task CancelSaleAsync(int saleId);
    }
}