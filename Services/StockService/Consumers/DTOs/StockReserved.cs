using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockService.Consumers.DTOs
{
    internal record StockReserved(int SaleId, int ProductId, int Quantity);
}