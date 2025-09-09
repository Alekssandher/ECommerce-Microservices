namespace SalesService.DTOs
{
    public class ItemResponse
    {
        public int ProductId { get; set; }  
        public int Quantity { get; set; }  
        public decimal UnitPrice { get; set; }  
        public decimal TotalPrice => Math.Round(Quantity * UnitPrice, 2);
    }
}