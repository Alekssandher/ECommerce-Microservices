using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StockService.Models
{
    public class StockItem
    {
        [Key]
        public int ProductId { get; set; }
        public int QuantityAvailable { get; set; }
        public int QuantityReserved { get; set; }    
        
        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; } = default!;
    }
}