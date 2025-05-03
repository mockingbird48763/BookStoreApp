using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.DTO.Request
{
    public class CreateOrderItem
    {
        public int BookId { get; set; }
        public int Quantity { get; set; }
    }
}
