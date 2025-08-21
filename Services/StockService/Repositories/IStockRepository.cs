using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StockService.Models;

namespace StockService.Repositories
{
    public interface IStockRepository
    {
        Task ReserveStockAsync(StockItem stockItem, int quantity);
        Task ReleaseStockAsync(StockItem stockItem, int quantity);       
        Task RemoveStockAsync(StockItem stockItem, int quantity);  
        Task<int> GetStockByProductIdAsync(int productId); 
    }
}