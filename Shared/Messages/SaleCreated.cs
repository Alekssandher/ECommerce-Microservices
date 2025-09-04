
namespace Shared.Messages
{
    public record SaleCreated(int SaleId, int CustomerId, List<ItemReserved> Items) ;
}