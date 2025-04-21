using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.DTO.Response
{
    public class OrderDetailItemDto
    {
        public int Id { get; set; }
        public required decimal UnitPrice { get; set; }
        public required int Quantity { get; set; }
        public required int BookId { get; set; }
        public required string BookName { get; set; }
    }
}
