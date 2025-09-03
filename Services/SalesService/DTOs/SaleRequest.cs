using System.ComponentModel.DataAnnotations;

namespace SalesService.DTOs
{
    public class SaleRequest
    {
        [Required]
        public required int CustomerId { get; init; }

        public List<ItemRequest> Items { get; init; } = [];
        
    }
}