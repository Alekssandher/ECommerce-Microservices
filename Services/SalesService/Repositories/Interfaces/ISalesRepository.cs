using SalesService.Models;

namespace SalesService.Repositories.Interfaces
{
    internal interface ISalesRepository
    {
        Task<Sale?> GetByIdAsync(int id);
        Task<List<Sale>> GetAllAsync();
        Task AddAsync(Sale sale);
    }
}