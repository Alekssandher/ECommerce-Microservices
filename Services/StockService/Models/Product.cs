using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace StockService.Models
{
    internal class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; init; }

        [StringLength(200)]
        public required string Name { get; init; }

        [StringLength(2000)]
        public required string Description { get; init; }

        public required decimal Price { get; init; }

        public required int Quantity { get; init; }
        

    }
}