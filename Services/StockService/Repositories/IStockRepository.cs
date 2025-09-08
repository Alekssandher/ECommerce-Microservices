using StockService.Models;


namespace StockService.Repositories
{
    public interface IStockRepository
    {
        Task ReserveStockAsync(StockItem stockItem, int quantity);
        Task ReserveStockItemsAsync(List<(int productId, int quantity)> items);
        Task ReleaseReservedStockAsync(StockItem stockItem, int quantity);       
        Task RemoveReservedStockAsync(StockItem stockItem, int quantity);
        Task<StockItem?> GetStockItemByProductIdAsync(int productId);
        Task<int> GetAvailableQuantityAsync(int productId); 
    }
}