using SalesService.Models;

namespace SalesService.Repositories.Interfaces
{
    internal interface ISalesRepository
    {
        Task<Sale> CreateSaleAsync(Sale sale);
        Task CancelSaleAsync(int saleId, int usid);
        Task UnauthorizeSale(Sale sale);
        Task DeleteSaleAsync(Sale sale);
        Task ConfirmSaleAsync(Sale sale);
        Task<Sale?> GetByIdAsync(int id, int usid);
        Task<List<Sale>> GetAllAsync(int usid);
        Task AddAsync(Sale sale);
    }
}