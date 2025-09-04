using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shared.Exceptions;
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

        public async Task ReserveStockItemsAsync(List<(int productId, int quantity)> items)
        {
            var productIds = items.Select(i => i.productId).ToList();

            var stockItems = await _context.Stocks
                .Include(s => s.Product)
                .Where(s => productIds.Contains(s.ProductId))
                .ToListAsync();

            var missingProducts = productIds.Except(stockItems.Select(s => s.ProductId)).ToList();
            
            if (missingProducts.Count != 0)
                throw new Exceptions.NotFoundException($"Products not found: {string.Join(", ", missingProducts)}");

            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                foreach (var (productId, quantity) in items)
                {
                    var stockItem = stockItems.FirstOrDefault(s => s.ProductId == productId)
                        ?? throw new Exceptions.NotFoundException($"Not Found Product With Id: {productId}");

                    if (stockItem.QuantityAvailable < quantity)
                        throw new Exceptions.BadRequestException($"Not enough stock for Product {productId}");

                    stockItem.QuantityAvailable -= quantity;
                    stockItem.QuantityReserved += quantity;

                    _context.Stocks.Update(stockItem);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}