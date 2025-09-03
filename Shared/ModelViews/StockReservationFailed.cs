using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shared.ModelViews
{
    public record StockReservationFailed
    (
        int ProductId,
        int Quantity,
        string Reason
    );
}