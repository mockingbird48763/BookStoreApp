using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Models
{
    public class Book
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "ISBN is required.")]
        [StringLength(16, MinimumLength = 16, ErrorMessage = "ISBN must be exactly 16 characters.")]
        [RegularExpression(@"^\d{3}-\d-\d{6}-\d{3}$", ErrorMessage = "ISBN must be in the format '978-1-234567-910'.")]
        public required string Isbn { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(50, ErrorMessage = "Title cannot be longer than 50 characters.")]
        public required string Title { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(200, ErrorMessage = "Description cannot be longer than 200 characters.")]
        public required string Description { get; set; }

        [Required(ErrorMessage = "List price is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "List price must be greater than 0.")]
        public required decimal ListPrice { get; set; }

        [Required(ErrorMessage = "Discount is required.")]
        [Range(0, 100, ErrorMessage = "Discount must be between 0 and 100.")]
        public required short Discount { get; set; }

        [Required(ErrorMessage = "Stock is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Stock must be greater than 0.")]
        public required int Stock { get; set; }

        [Required(ErrorMessage = "Publication date is required.")]
        public required DateOnly PublicationDate { get; set; }

        [Required(ErrorMessage = "Image path is required.")]
        [StringLength(50, ErrorMessage = "ImagePath cannot be longer than 50 characters.")]
        public required string ImagePath { get; set; }

        [Required(ErrorMessage = "Author is required.")]
        public required Author Author { get; set; }

        [Required(ErrorMessage = "Publisher is required.")]
        public required Publisher Publisher { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; } = [];
    }
}
