using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Services.Models;

namespace SalesService.Models
{
    public class Sale
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; init; }

        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

        public int CustomerId { get; set; } 

        public SaleStatus Status { get; set; } = SaleStatus.Pending;

        public List<SaleItem> Items { get; set; } = [];
    }

    public enum SaleStatus
    {
        Pending,
        Confirmed,
        NotAllowed,
        Canceled
    }
}