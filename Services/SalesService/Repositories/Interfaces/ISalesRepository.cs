using SalesService.Models;

namespace SalesService.Repositories.Interfaces
{
    internal interface ISalesRepository
    {
        Task<Sale> CreateSaleAsync(Sale sale);
        Task UnauthorizeSale(Sale sale);
        Task DeleteSaleAsync(Sale sale);
        Task ConfirmSaleAsync(Sale sale);
        Task<Sale?> GetByIdAsync(int id);
        Task<List<Sale>> GetAllAsync();
        Task AddAsync(Sale sale);
    }
}