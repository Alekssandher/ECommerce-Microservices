using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StockService.Infraestructure.Data;
using StockService.Models;

namespace StockService.Repositories
{
    internal class StockRepository : IStockRepository
    {
        private readonly StockContext _stockContext;
        public StockRepository(StockContext stockContext)
        {
            _stockContext = stockContext;
        }

        public async Task<int> GetStockByProductIdAsync(int productId)
        {
            var res = await _stockContext.Stocks.FirstOrDefaultAsync(s => s.ProductId == productId);

            return res?.QuantityAvailable ?? 0;
        }

        public async Task ReleaseStockAsync(StockItem stockItem, int quantity)
        {
            stockItem.QuantityAvailable += quantity;  
            stockItem.QuantityReserved -= quantity;

            await _stockContext.SaveChangesAsync();
        }

        public async Task RemoveStockAsync(StockItem stockItem, int quantity)
        {
            stockItem.QuantityReserved -= quantity;   
    
            await _stockContext.SaveChangesAsync();
        }

        public async Task ReserveStockAsync(StockItem stockItem, int quantity)
        {
            stockItem.QuantityAvailable -= quantity;  
            stockItem.QuantityReserved += quantity;  

            await _stockContext.SaveChangesAsync();
        }
    }
}