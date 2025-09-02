using SalesService.Models;

namespace SalesService.DTOs
{
    public class SaleResponse
    {
        public int Id { get; set; }              
        public int CustomerId { get; set; }      
        public SaleStatus Status { get; set; }       
        public DateTime CreatedAt { get; set; }  
        public List<ItemResponse> Items { get; set; } = new();
        public decimal TotalAmount { get; set; } 
    }
}