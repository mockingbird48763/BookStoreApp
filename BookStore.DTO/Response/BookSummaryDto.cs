using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.DTO.Response
{
    public class BookSummaryDto
    {
        public required int Id { get; set; }
        public required string Title { get; set; }
        public decimal ListPrice { get; set; }
        public decimal Discount { get; set; }
        public required string ImagePath { get; set; }
    }
}
