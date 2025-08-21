using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockService.Services
{
    public interface IStockService
    {
        Task ReserveStock(int productId, int quantity); 
        Task ReleaseStock(int productId, int quantity);       
        Task RemoveStock(int productId, int quantity);  
        Task<int> GetStockByProductId(int productId); 
    }
}