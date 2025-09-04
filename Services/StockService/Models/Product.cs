using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StockService.Models
{
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; init; }

        [StringLength(200)]
        public required string Name { get; set; }

        [StringLength(2000)]
        public required string Description { get; set; }

        public required decimal Price { get; set; }       

    }
}