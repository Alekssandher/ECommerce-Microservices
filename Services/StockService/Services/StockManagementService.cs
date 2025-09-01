using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StockService.Repositories;

namespace StockService.Services
{
    public class StockManagementService : IStockManagementService
    {
        private readonly IStockRepository _stockRepository;
        private readonly IProductRepository _productRepository;

        public StockManagementService(IStockRepository stockRepository, IProductRepository productRepository)
        {
            _stockRepository = stockRepository;
            _productRepository = productRepository;
        }

        public async Task<int> GetAvailableStockAsync(int productId)
        {
            // Verificar se o produto existe
            _ = await _productRepository.GetProductByIdAsync(productId) 
                ?? throw new ArgumentException($"Product {productId} not found");
            
            return await _stockRepository.GetAvailableQuantityAsync(productId);
        }

        public async Task ReserveStockAsync(int productId, int quantity)
        {
            var stockItem = await _stockRepository.GetStockItemByProductIdAsync(productId) 
                ?? throw new ArgumentException($"Stock for product {productId} not found");

            if (stockItem.QuantityAvailable < quantity)
                throw new InvalidOperationException($"Insufficient stock. Available: {stockItem.QuantityAvailable}, Requested: {quantity}");

            await _stockRepository.ReserveStockAsync(stockItem, quantity);
        }

        public async Task ReleaseStockAsync(int productId, int quantity)
        {
            var stockItem = await _stockRepository.GetStockItemByProductIdAsync(productId)
                ?? throw new ArgumentException($"Stock for product {productId} not found");

            if (stockItem.QuantityReserved < quantity)
                throw new InvalidOperationException($"Cannot release more than reserved. Reserved: {stockItem.QuantityReserved}, Requested: {quantity}");

            await _stockRepository.ReleaseReservedStockAsync(stockItem, quantity);
        }

        public async Task RemoveStockAsync(int productId, int quantity)
        {
            var stockItem = await _stockRepository.GetStockItemByProductIdAsync(productId)
                ?? throw new ArgumentException($"Stock for product {productId} not found");

            if (stockItem.QuantityReserved < quantity)
                throw new InvalidOperationException($"Cannot remove more than reserved. Reserved: {stockItem.QuantityReserved}, Requested: {quantity}");

            await _stockRepository.RemoveReservedStockAsync(stockItem, quantity);
        }
    }
}