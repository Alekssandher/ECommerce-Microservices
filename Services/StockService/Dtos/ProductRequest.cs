using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StockService.Dtos
{
    internal class ProductRequest
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