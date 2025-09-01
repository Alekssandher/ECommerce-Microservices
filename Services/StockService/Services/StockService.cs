using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StockService.Repositories;

namespace StockService.Services
{
    public class StockService : IStockService
    {
        private readonly IStockRepository _stockRepository;
        private readonly IProductRepository _productRepository;

        public StockService(IStockRepository stockRepository, IProductRepository productRepository)
        {
            _stockRepository = stockRepository;
            _productRepository = productRepository;
        }

        public async Task<int> GetStockByProductId(int productId)
        {
            _ = await _productRepository.GetStockByProductIdAsync(productId) 
                ?? throw new ArgumentException($"Produto {productId} não encontrado.");
            
            return await _stockRepository.GetStockQuantityByProductIdAsync(productId);
        }

        public async Task ReserveStock(int productId, int quantity)
        {
            var stockItem = await _stockRepository.GetStockByProductIdAsync(productId) 
                ?? throw new ArgumentException($"Produto {productId} não encontrado.");

            await _stockRepository.ReserveStockAsync(stockItem, quantity);
        }

        public async Task ReleaseStock(int productId, int quantity)
        {
            var product = await _productRepository.GetStockByProductIdAsync(productId) 
                ?? throw new ArgumentException($"Produto {productId} não encontrado.");

            var stockItem = await _stockRepository.GetStockByProductIdAsync(productId);
                

            await _stockRepository.ReleaseStockAsync(stockItem, quantity);
        }

        public async Task RemoveStock(int productId, int quantity)
        {
            var stockItem = await _stockRepository.GetStockByProductIdAsync(productId)
                ?? throw new ArgumentException($"Produto {productId} não encontrado.");
            await _stockRepository.RemoveStockAsync(stockItem, quantity);
        }
    }

}