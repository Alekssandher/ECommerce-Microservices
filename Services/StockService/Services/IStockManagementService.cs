using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StockService.Dtos;
using StockService.Models;

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