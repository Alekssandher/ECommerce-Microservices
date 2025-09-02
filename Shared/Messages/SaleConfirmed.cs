using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shared.Messages
{
    public record SaleConfirmed(int CustomerId, int StockItemId, int Quantity);
}