using SalesService.Models;

namespace SalesService.DTOs
{
    public class SaleResponse
    {
        public int Id { get; set; }              
        public int CustomerId { get; set; }
        public string Status { get; set; } = default!;      
        public DateTime CreatedAt { get; set; }  
        public List<ItemResponse> Items { get; set; } = [];
        public decimal TotalAmount => Items.Sum(i => i.Quantity * i.UnitPrice);
    }
}