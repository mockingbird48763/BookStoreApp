using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.DTO.Response
{
    public class BookSummaryForManagementDto
    {
        public required int Id { get; set; }
        public required string ImagePath { get; set; }
        public required string Title { get; set; }
        public required decimal ListPrice { get; set; }
        public required decimal Discount { get; set; }
        public required decimal UnitPrice { get; set; }
        public required int Stock { get; set; }
        public required bool IsVisible { get; set; }
    }
}
