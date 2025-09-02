using SalesService.Models;

namespace SalesService.Repositories.Interfaces
{
    internal interface ISalesRepository
    {
        Task CreateSaleAsync(Sale sale);
        Task ConfirmSaleAsync(Sale sale);
        Task<Sale?> GetByIdAsync(int id);
        Task<List<Sale>> GetAllAsync();
        Task AddAsync(Sale sale);
    }
}