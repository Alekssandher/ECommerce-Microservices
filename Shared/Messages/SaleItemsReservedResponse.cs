using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shared.Messages
{
    public class SaleItemsReservedResponse
    {
        public int CustomerId { get; init; }
        public List<ItemReserved> ItemsReserved { get; init; } = default!;
    }

    public class ItemReserved {
        public int SaleId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}