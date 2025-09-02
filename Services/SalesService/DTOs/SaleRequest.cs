namespace SalesService.DTOs
{
    internal class SaleRequest
    {
        public int CustomerId { get; init; }

        public List<ItemRequest> Items { get; init; } = [];
        
    }
}