using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace StockService.Models
{
    public class StockItem
    {
        [Key]
        public int ProductId { get; set; }
        public int QuantityAvailable { get; set; }    
        public int QuantityReserved { get; set; }    
    }
}