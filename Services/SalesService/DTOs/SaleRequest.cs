namespace SalesService.DTOs
{
    public class SaleRequest
    {
        public int CustomerId { get; init; }

        public List<ItemRequest> Items { get; init; } = [];
        
    }
}