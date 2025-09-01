using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shared.ModelViews
{
    public class StockReleased
    {
        public int StockItemId { get; set; }
        public int Quantity { get; set; }
        public DateTime ReleasedAt { get; set; }
    }

}