using System.ComponentModel.DataAnnotations;

namespace StockService.Dtos
{
    public class ProductRequest
    {
        [Required]
        [StringLength(200)]
        public required string Name { get; init; }

        [Required]
        [StringLength(2000)]
        public required string Description { get; init; }

        [Required]
        public required decimal Price { get; init; }

        [Required]
        public required int Quantity { get; init; }
    }
}