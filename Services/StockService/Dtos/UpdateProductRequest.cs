using System.ComponentModel.DataAnnotations;

namespace StockService.Dtos
{
    public class UpdateProductRequest
    {
        [Required]
        public required int ProductId { get; init; }
        
        [StringLength(200)]
        public string? Name { get; init; }

        [StringLength(2000)]
        public string? Description { get; init; }
      
        public decimal? Price { get; init; }

        public int? Quantity { get; init; }
    }
}