using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockService.Dtos
{
    public class StockResponse
    {
        public int ProductId { get; init; }
        public int QuantityAvailable { get; init; }
        public int QuantityReserved { get; init; }
    }
}