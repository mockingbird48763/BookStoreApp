using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.DTO.Response
{
    public class CartBookItemDto
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public required decimal UnitPrice { get; set; }

        public required int Stock { get; set; }

        public required string ImagePath { get; set; }
    }
}
