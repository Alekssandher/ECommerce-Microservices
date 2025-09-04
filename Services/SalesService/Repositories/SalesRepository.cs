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

        public async Task ConfirmSaleAsync(Sale sale)
        {
            _ = await _salesContext.Sales
                .Where(s => s.Id == sale.Id)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(p => p.Status, SaleStatus.Confirmed)
                );

        }

        public async Task UnauthorizeSale(Sale sale)
        {
            _ = await _salesContext.Sales
                .Where(s => s.Id == sale.Id)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(p => p.Status, SaleStatus.NotAllowed)
                ); 
        }

        public async Task<Sale> CreateSaleAsync(Sale sale)
        {
            var res = await _salesContext.Sales.AddAsync(sale);
            await _salesContext.SaveChangesAsync();

            return res.Entity;
        }

        public async Task DeleteSaleAsync(Sale sale)
        {
            _salesContext.Sales.Remove(sale);
            await _salesContext.SaveChangesAsync();
        }

        public async Task<List<Sale>> GetAllAsync()
        {
            return await _salesContext.Sales
                .Include(s => s.Items)
                .ToListAsync();
        }

        public async Task<Sale?> GetByIdAsync(int id)
        {
            return await _salesContext.Sales.FirstOrDefaultAsync(s => s.Id == id);
        }
    }
}