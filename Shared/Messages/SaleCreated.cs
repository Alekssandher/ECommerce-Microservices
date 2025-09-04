
namespace Shared.Messages
{
    public record SaleCreated(int CustomerId, List<ItemReserved> Items) ;
}