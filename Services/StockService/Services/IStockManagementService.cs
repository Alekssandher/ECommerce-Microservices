using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockService.Services
{
    public interface IStockManagementService
    {
        Task ReserveStockAsync(int productId, int quantity); 
        Task ReleaseStockAsync(int productId, int quantity);       
        Task RemoveStockAsync(int productId, int quantity);  
        Task<int> GetAvailableStockAsync(int productId); 
    }
}