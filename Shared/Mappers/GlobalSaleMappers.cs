using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Shared.Messages;

namespace Shared.Mappers
{
    public static class GlobalSaleMappers
    {
        public static SaleItemsReservedResponse ToReservedResponse(this SaleCreated request)
        {
            return new SaleItemsReservedResponse
            {
                SaleId = request.SaleId,
                CustomerId = request.CustomerId,
                ItemsReserved = request.Items
            };
        }
    }
}