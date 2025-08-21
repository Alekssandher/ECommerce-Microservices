using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StockService.Dtos
{
    public class ProductUpdateRequest
    {
        [Required]
        public required int Id { get; init; }
        
        [StringLength(200)]
        public string? Name { get; init; } = default!;

        [StringLength(2000)]
        public string? Description { get; init; } = default!;
      
        public decimal? Price { get; init; } = default!;

        public int? Quantity { get; init; } = default!;
    }
}