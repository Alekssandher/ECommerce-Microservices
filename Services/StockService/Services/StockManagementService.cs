using MassTransit;
using Shared.Exceptions;
using StockService.Dtos;
using StockService.Repositories;

namespace StockService.Services
{
    public class StockManagementService : IStockManagementService
    {
        private readonly IStockRepository _stockRepository;
        private readonly IProductRepository _productRepository;
        private readonly IPublishEndpoint _publishEndpoint;

        public StockManagementService(IStockRepository stockRepository, IProductRepository productRepository, IPublishEndpoint publishEndpoint)
        {
            _stockRepository = stockRepository;
            _productRepository = productRepository;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<int> GetAvailableStockAsync(int productId)
        {
            
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

        public async Task ReserveStockItemsAsync(List<(int productId, int quantity)> items)
        {
          
            await _stockRepository.ReserveStockItemsAsync(items);
             
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

        public async Task<StockResponse> GetStockByIdAsync(int stockId)
        {
            var stockItem = await _stockRepository.GetStockItemByProductIdAsync(stockId)
                ?? throw new Exceptions.NotFoundException("There's no item with this ID");

            return new StockResponse
            {
                ProductId = stockItem.ProductId,
                QuantityAvailable = stockItem.QuantityAvailable,
                QuantityReserved = stockItem.QuantityReserved
            };

        }
    }
}