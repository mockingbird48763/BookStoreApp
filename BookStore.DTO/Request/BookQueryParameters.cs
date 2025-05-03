using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.DTO.Request
{
    public class BookQueryParameters
    {
        [Range(1, int.MaxValue, ErrorMessage = "Page must be greater than 0.")]
        public int Page { get; set; } = 1;

        [Range(1, int.MaxValue, ErrorMessage = "PageSize must be greater than 0.")]
        public int PageSize { get; set; } = 10;

        [Range(1, int.MaxValue, ErrorMessage = "AuthorId must be greater than 0.")]
        public int? AuthorId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "PublisherId must be greater than 0.")]
        public int? PublisherId { get; set; }

        [StringLength(10, ErrorMessage = "Keyword cannot be longer than 10 characters.")]
        public string? Keyword { get; set; }
    }
}
