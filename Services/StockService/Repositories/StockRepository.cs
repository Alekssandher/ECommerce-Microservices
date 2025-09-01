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
        private readonly StockContext _context;
        
        public StockRepository(StockContext context)
        {
            _context = context;
        }

        public async Task<StockItem?> GetStockItemByProductIdAsync(int productId)
        {
            return await _context.Stocks.FirstOrDefaultAsync(s => s.ProductId == productId);
        }

        public async Task<int> GetAvailableQuantityAsync(int productId)
        {
            var stockItem = await _context.Stocks.FirstOrDefaultAsync(s => s.ProductId == productId);
            return stockItem?.QuantityAvailable ?? 0;
        }

        public async Task ReleaseReservedStockAsync(StockItem stockItem, int quantity)
        {
            stockItem.QuantityAvailable += quantity;  
            stockItem.QuantityReserved -= quantity;
            await _context.SaveChangesAsync();
        }

        public async Task RemoveReservedStockAsync(StockItem stockItem, int quantity)
        {
            stockItem.QuantityReserved -= quantity;   
            await _context.SaveChangesAsync();
        }

        public async Task ReserveStockAsync(StockItem stockItem, int quantity)
        {
            stockItem.QuantityAvailable -= quantity;  
            stockItem.QuantityReserved += quantity;  
            await _context.SaveChangesAsync();
        }
    }
}