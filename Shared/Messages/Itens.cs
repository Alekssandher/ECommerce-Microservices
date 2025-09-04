using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shared.Messages
{
    public class Itens
    {
        public record ValidateSaleItems(int SaleId, int CustomerId, List<int> ItemsId);

        public record SaleItemsValidated(int SaleId, List<ItemValidated> Items);

        public record ItemValidated(int ProductId, int Quantity, decimal Price);
    }
}