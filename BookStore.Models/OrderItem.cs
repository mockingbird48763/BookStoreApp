using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Models
{
    public class OrderItem
    {
        public int Id { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "List Unit Price must be greater than 1.")]
        public decimal UnitPrice { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "List Price must be greater than 1.")]
        public int Quantity { get; set; }

        public required Book Book { get; set; }
    }
}
