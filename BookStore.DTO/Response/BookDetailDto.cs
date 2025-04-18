using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.DTO.Response
{
    public class BookDetailDto
    {
        public int Id { get; set; }
        public required string Isbn { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required decimal ListPrice { get; set; }
        public required short Discount { get; set; }
        public required int Stock { get; set; }
        public required DateOnly PublicationDate { get; set; }
        public required string ImagePath { get; set; }
        public required int AuthorId { get; set; }
        public required string AuthorName { get; set; }
        public required int PublisherId { get; set; }
        public required string PublisherName { get; set; }
    }
}
