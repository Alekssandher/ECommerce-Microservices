using StockService.Dtos;

namespace StockService.Services
{
    public interface IStockManagementService
    {
        Task ReserveStockAsync(int productId, int quantity);
        Task ReserveStockItemsAsync(List<(int productId, int quantity)> items);
        Task ReleaseStockAsync(int productId, int quantity);
        Task RemoveStockAsync(int productId, int quantity);
        Task<int> GetAvailableStockAsync(int productId);
        Task<StockResponse> GetStockByIdAsync(int productId);
    }
}