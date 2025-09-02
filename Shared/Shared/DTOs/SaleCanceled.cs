using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shared.DTOs
{
    public class SaleCanceled
    {
        public int StockItemId { get; set; }
        public int Quantity { get; set; }
        public DateTime Canceled { get; set; }
    }
}