using SalesService.Repositories.Interfaces;
using SalesService.Models;
using SalesService.Infraestructure.Data;
using Microsoft.EntityFrameworkCore;

namespace SalesService.Repositories
{
    internal class SalesRepository : ISalesRepository
    {
        private readonly SalesContext _salesContext;

        public SalesRepository(SalesContext salesContext)
        {
            _salesContext = salesContext;
        }

        public async Task AddAsync(Sale sale)
        {
            await _salesContext.AddAsync(sale);
            await _salesContext.SaveChangesAsync();

        }

        public async Task<List<Sale>> GetAllAsync()
        {
            return await _salesContext.Sales.ToListAsync();
        }

        public async Task<Sale?> GetByIdAsync(int id)
        {
            return await _salesContext.Sales.FirstOrDefaultAsync(s => s.Id == id);
        }
    }
}