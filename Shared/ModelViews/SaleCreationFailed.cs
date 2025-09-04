using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shared.ModelViews
{
    public record SaleCreationFailed
    (
        int CustomerId,
        int SaleId,
        string Reason
    );
}