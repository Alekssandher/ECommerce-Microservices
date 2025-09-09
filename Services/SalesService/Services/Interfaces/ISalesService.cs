using SalesService.DTOs;
using Shared.Messages;

namespace SalesService.Services.Interfaces
{
    public interface ISalesService
    {
        Task<int> CreateSaleAsync(SaleItemsReservedResponse request);
        Task UnauthorizeSale(int saleId);
        Task SendSaleAsync(SaleRequest request);
        Task<SaleResponse> GetSaleByIdAsync(int saleId);
        Task<List<SaleResponse>> GetAllSalesAsync();
        Task ConfirmSaleAsync(int saleId);
        Task CancelSaleAsync(int saleId);
    }
}