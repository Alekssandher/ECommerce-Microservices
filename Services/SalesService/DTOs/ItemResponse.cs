using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SalesService.DTOs
{
    public class ItemResponse
    {
        public int ProductId { get; set; }  
        public int Quantity { get; set; }  
        public decimal Price { get; set; }  
        public decimal Total => Quantity * Price;
    }
}